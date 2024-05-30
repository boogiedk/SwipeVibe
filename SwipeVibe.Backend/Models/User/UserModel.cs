namespace SwipeVibe.Backend.Models.User;

public class UserModel
{
    public Guid UserId { get; set; }
    public required string Msisdn { get; set; }
    public required string PasswordHash { get; set; }
}