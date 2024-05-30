namespace SwipeVibe.Backend.Models.Profile;

public class ProfileResponse
{
    public Guid ProfileId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime BirthdayDate { get; set; }
    public Gender Gender { get; set; }
    public string? Description { get; set; }
    public string? CityName { get; set; }
}