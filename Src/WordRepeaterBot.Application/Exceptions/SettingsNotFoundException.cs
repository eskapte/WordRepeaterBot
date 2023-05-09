namespace WordRepeaterBot.Application.Exceptions;

public class SettingsNotFoundException : Exception
{
    public SettingsNotFoundException(long userId) 
        : base($"Settings not found for user with id {userId}") { }

    public static void ThrowIfNull(DataAccess.Models.Settings? settings, long userId)
    {
        if (settings is null)
        {
            throw new SettingsNotFoundException(userId);
        }
    }
}
