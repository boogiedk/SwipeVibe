namespace SwipeVibe.Backend.Models.User;

public class UserSearchFilter
{
    public string FirstName { get; set; }
    public string SecondName { get; set; }

    public static bool TryParse(string value, out UserSearchFilter result)
    {
        result = new UserSearchFilter();

        var parts = value.Split(';');
        if (parts.Length >= 2)
        {
            result.FirstName = parts[0];
            result.SecondName = parts[1];
            return true;
        }

        return false;
    }
}