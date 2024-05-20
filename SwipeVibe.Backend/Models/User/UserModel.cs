namespace SwipeVibe.Backend.Models.User;

public class UserModel
{
    public Guid UserId { get; set; }
    public string Msisdn { get; set; }
    public string PasswordHash { get; set; }
}