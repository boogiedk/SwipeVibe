namespace SwipeVibe.Backend.Models.Database;

public class UserModelDb
{
    public Guid UserId { get; set; }
    public string? Msisdn { get; set; }
    public string? PasswordHash { get; set; }
    public ProfileModelDb? Profile { get; set; }
}