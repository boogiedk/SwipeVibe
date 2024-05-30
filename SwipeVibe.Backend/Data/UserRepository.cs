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
    Task<List<ProfileModelDb>> GetProfilesByFilter(string firstName, string lastName);
}


public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task CreateUser(UserModelDb user)
    {
        await context.Database.ExecuteSqlInterpolatedAsync(@$"
        INSERT INTO ""Users"" 
            (
                ""UserId"", 
                ""Msisdn"", 
                ""PasswordHash""
            )
        VALUES
            (
                {user.UserId}, 
                {user.Msisdn}, 
                {user.PasswordHash}
            )");
        await context.SaveChangesAsync();
    }

    public async Task CreateProfile(ProfileModelDb profile)
    {
        await context.Database.ExecuteSqlInterpolatedAsync(@$"
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
                {profile.ProfileId}, 
                {profile.UserId}, 
                {profile.FirstName}, 
                {profile.LastName}, 
                {profile.BirthdayDate}, 
                {profile.Gender}, 
                {profile.Description}, 
                {profile.CityName}
            )");
        
        await context.SaveChangesAsync();
    }
    
    public async Task<UserModelDb?> GetUserByMsisdn(string msisdn)
    {
        return await context.Users!
            .FromSqlInterpolated(@$"
                SELECT 
                    ""UserId"",
                    ""Msisdn"",
                    ""PasswordHash""
                FROM ""Users"" 
                WHERE ""Msisdn"" = {msisdn}")
            .FirstOrDefaultAsync();
    }

    public async Task<ProfileModelDb?> GetProfileByUserId(Guid userId)
    {
        return await context.Profiles
            .FromSqlInterpolated(@$"
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
                WHERE p.""UserId"" = {userId}")
            .FirstOrDefaultAsync();
    }

    public async Task<List<ProfileModelDb>> GetProfilesByFilter(string firstName, string lastName)
    {
        return await context.Profiles.FromSqlInterpolated(@$"
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
                (
                    p.""FirstName"" LIKE '%' || {firstName} || '%'
                    OR 
                    p.""LastName"" LIKE '%' || {lastName} || '%'
                )")
            .ToListAsync();
    }
}
