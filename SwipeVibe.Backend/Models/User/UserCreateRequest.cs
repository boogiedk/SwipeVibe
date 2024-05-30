using SwipeVibe.Backend.Models.Profile;

namespace SwipeVibe.Backend.Models.User;

public class UserCreateRequest
{
    public required string Msisdn { get; set; }
    public required string Password { get; set; }
    public required ProfileCreateRequest Profile { get; set; }
}