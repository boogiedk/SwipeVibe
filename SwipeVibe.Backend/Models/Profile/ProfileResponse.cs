namespace SwipeVibe.Backend.Models.Profile;

public class ProfileResponse
{
    public Guid ProfileId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateTime BirthdayDate { get; set; }
    public Gender Gender { get; set; }
    public required string Description { get; set; }
    public required string CityName { get; set; }
}