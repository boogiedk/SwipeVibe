namespace SwipeVibe.Backend.Models.User;

public class UserLoginRequest
{
    public required string Msisdn { get; set; }
    public required string Password { get; set; }
}