using Microsoft.EntityFrameworkCore;
using SwipeVibe.Backend.Infrastructure;
using SwipeVibe.Backend.Models.Database;

namespace SwipeVibe.Backend.Data;

public interface IUserRepository
{
    Task CreateUser(UserModelDb user);
    Task CreateProfile(ProfileModelDb profile);
    Task<UserModelDb?> GetUserByMsisdn(string msisdn);
    Task<ProfileModelDb?> GetProfileByUserId(Guid profileId);
    Task<List<ProfileModelDb?>> GetProfilesByUserId(Guid userId, string firstName, string lastName);
}


public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task CreateUser(UserModelDb user)
    {
        var sql = @"
        INSERT INTO ""Users"" 
            (
            ""UserId"", 
            ""Msisdn"", 
            ""PasswordHash""
            )
        VALUES
            (
            {0}, 
            {1}, 
            {2}
            )";

        await context.Database.ExecuteSqlRawAsync(sql, user.UserId, user.Msisdn, user.PasswordHash);
        await context.SaveChangesAsync();
    }

    public async Task CreateProfile(ProfileModelDb profile)
    {
        var sql = @"
        INSERT INTO ""Profiles"" 
            (
             ""ProfileId"", 
             ""UserId"", 
             ""FirstName"", 
             ""LastName"", 
             ""BirthdayDate"", 
             ""Gender"", 
             ""Description"", 
             ""CityName""
             )
        VALUES 
            (
                {0}, 
                {1}, 
                {2}, 
                {3}, 
                {4}, 
                {5}, 
                {6}, 
                {7}
            )";

        await context.Database.ExecuteSqlRawAsync(sql,
            profile.ProfileId,
            profile.UserId, profile.FirstName,
            profile.LastName,
            profile.BirthdayDate,
            profile.Gender,
            profile.Description,
            profile.CityName);
        await context.SaveChangesAsync();
    }
    
    public async Task<UserModelDb?> GetUserByMsisdn(string msisdn)
    {
        return await context.Users!
            .FromSqlRaw(@"
                SELECT 
                    ""UserId"",
                    ""Msisdn"",
                    ""PasswordHash""
                FROM ""Users"" 
                WHERE ""Msisdn"" = {0}", msisdn)
            .FirstOrDefaultAsync();
    }

    public async Task<ProfileModelDb?> GetProfileByUserId(Guid userId)
    {
        return await context.Profiles
            .FromSqlRaw(@"
                SELECT 
                    p.""ProfileId"",
                    p.""UserId"", 
                    p.""FirstName"", 
                    p.""LastName"", 
                    p.""BirthdayDate"", 
                    p.""Gender"",
                    p.""Description"", 
                    p.""CityName"" 
                FROM ""Profiles"" p
                WHERE p.""UserId"" = {0}", userId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ProfileModelDb?>> GetProfilesByUserId(Guid userId, string firstName, string lastName)
    {
        var profiles = await context.Profiles.FromSqlRaw(@"
                SELECT 
                    p.""ProfileId"",
                    p.""UserId"", 
                    p.""FirstName"", 
                    p.""LastName"", 
                    p.""BirthdayDate"", 
                    p.""Gender"",
                    p.""Description"", 
                    p.""CityName"" 
                FROM ""Profiles"" p
                WHERE 
                    p.""UserId"" != {0}
                    AND 
                        (
                            p.""FirstName"" = {1} OR p.""LastName"" = {2}
                        )", userId, firstName, lastName)
            .ToListAsync();

        return profiles;
    }
}
