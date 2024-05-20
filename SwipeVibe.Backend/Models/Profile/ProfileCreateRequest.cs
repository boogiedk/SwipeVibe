namespace SwipeVibe.Backend.Models.Profile;

public class ProfileCreateRequest
{
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public DateTime BirthdayDate { get; set; }
    public Gender Gender { get; set; }
    public string Description { get; set; }
    public string CityName { get; set; }
}