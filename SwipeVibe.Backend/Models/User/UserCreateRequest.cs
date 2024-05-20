using SwipeVibe.Backend.Models.Profile;

namespace SwipeVibe.Backend.Models.User;

public class UserCreateRequest
{
    public string Msisdn { get; set; }
    public string Password { get; set; }
    public ProfileCreateRequest Profile { get; set; }
}