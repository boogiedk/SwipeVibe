using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SwipeVibe.Backend.Models.Profile;
using SwipeVibe.Backend.Models.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SwipeVibe.Backend.Infrastructure;
using SwipeVibe.Backend.Models.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var securityKey = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("SecurityKey").Value!);

# region Auth

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(securityKey)
        };
    });

# endregion

builder.Services.AddAuthorization(); 

# region Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите токен"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

# endregion

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

#region User Api

var usersGroup = app.MapGroup("api/v1/users");

usersGroup.MapPost("/", async (UserCreateRequest request, ApplicationDbContext context) =>
    {
        var user = await context.Users.FirstOrDefaultAsync(f => f.Msisdn == request.Msisdn);
        
        if (user is not null)
        {
            return Results.BadRequest();
        }
        
        var profileId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        await context.Users
            .AddAsync(new UserModelDb()
            {
                UserId = userId,
                Msisdn = request.Msisdn,
                PasswordHash = AuthExtension.ComputeMD5Hash(request.Password),
                Profile = new ProfileModelDb
                {
                    ProfileId = profileId,
                    UserId = userId,
                    FirstName = request.Profile.FirstName,
                    SecondName = request.Profile.SecondName,
                    CityName = request.Profile.CityName,
                    Description = request.Profile.Description,
                    BirthdayDate = request.Profile.BirthdayDate,
                    Gender = request.Profile.Gender
                }
            });

        await context.SaveChangesAsync();

        return Results.Ok(new UserCreateResponse()
        {
            UserId = userId
        });
    })
    .WithName("RegisterUser")
    .WithDescription("Создает пользователя")
    .WithOpenApi();


usersGroup.MapGet( "/{userId}", async (Guid userId, HttpRequest request, ApplicationDbContext context) =>
    {
        # region Auth

        var currentUser = await getCurrentUser(request, context);
        if (currentUser == null)
        {
            return Results.BadRequest("Incorrect token.");
        }
        
        # endregion

        var profile = await context.Profiles
            .Include(i => i.User)
            .FirstOrDefaultAsync(w => w.UserId == userId);
        
        if (profile is null)
        {
            return Results.NotFound();
        }

        if (profile.User.Msisdn != currentUser.Msisdn)
        {
            return Results.Unauthorized();
        }

        var result = new ProfileResponse()
        {
            ProfileId = profile.ProfileId,
            FirstName = profile.FirstName,
            SecondName = profile.SecondName,
            CityName = profile.CityName,
            Description = profile.Description,
            BirthdayDate = profile.BirthdayDate,
            Gender = profile.Gender
        };
        
        return Results.Ok(result);
    })
    .WithName("GetUser")
    .WithOpenApi()
    .WithDescription("Возвращает анкету пользователя")
    .RequireAuthorization();


usersGroup.MapPost( "/login",  async (UserLoginRequest request, ApplicationDbContext context) =>
    {
        var user = await context.Users.FirstOrDefaultAsync(f => f.Msisdn == request.Msisdn);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        if (user.PasswordHash != AuthExtension.ComputeMD5Hash(request.Password))
        {
            return Results.Unauthorized();
        }

        var userJwt = AuthExtension.GenerateJwt(user.Msisdn, securityKey);

        return Results.Ok(userJwt);
    })
    .WithName("LoginUser")
    .WithDescription("Авторизует пользователя")
    .WithOpenApi();

usersGroup.MapPost("/search", async (UserSearchFilter filter, HttpRequest request, ApplicationDbContext context) =>
    {
        var currentUser = await getCurrentUser(request, context);
        
        var profileModels = await context.Profiles
            .Where(w => w.FirstName.ToLower() == filter.FirstName.ToLower() || w.SecondName.ToLower() == filter.SecondName.ToLower())
            .Where(w=>w.User.Msisdn != currentUser.Msisdn)
            .Select(s=> new UserSearchResponse
            {
                ProfileId = s.ProfileId,
                FirstName = s.FirstName,
                SecondName = s.SecondName,
                Gender = s.Gender,
                BirthdayDate = s.BirthdayDate,
                CityName = s.CityName,
                Description = s.Description
            })
            .ToListAsync();

        return Results.Ok(profileModels);
    })
    .WithName("SearchUsers")
    .WithOpenApi()
    .WithDescription("Возвращает список анкет по заданному фильтру")
    .RequireAuthorization();

# endregion

async Task<UserModel?> getCurrentUser(HttpRequest request, ApplicationDbContext context)
{
    if (!request.Headers.TryGetValue("Authorization", out var authorizationHeader))
    {
        return null;
    }

    var token = authorizationHeader.ToString().Split(" ").Last();
    var handler = new JwtSecurityTokenHandler();

    if (!handler.CanReadToken(token))
    {
        return null;
    }

    var jwtToken = handler.ReadJwtToken(token);
    var msisdnClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "Msisdn")?.Value;

    var userModelDb = await context.Users.FirstOrDefaultAsync(w => w.Msisdn == msisdnClaim);

    return new UserModel()
    {
        UserId = userModelDb.UserId,
        PasswordHash = userModelDb.PasswordHash,
        Msisdn = userModelDb.Msisdn
    };
}

app.Run();
