namespace WordRepeaterBot.DataAccess.Models;

/// <summary>
/// Прочие настройки пользователя, к примеру, частота отправки сообщений
/// </summary>
public class Settings
{
    public long UserId { get; set; }
    public RepeatFrequencyInDay? FrequencePerDay { get; set; }
    public short TimeZoneOffset { get; set; }
    public SettingsSetupStep? SettingsSetupStep { get; set; }
}

public enum RepeatFrequencyInDay : byte
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Six = 6,
    Tvelwe = 12,
}

public enum SettingsSetupStep: byte
{
    SetTimeZone = 1,
    SetFrequence = 2
}

public static class SettingsSetupStepExtension
{
    public static SettingsSetupStep? NextStep(this SettingsSetupStep? step)
    {
        if (step is null)
            return null;

        step++;

        var isInRange = Enum.IsDefined(typeof(SettingsSetupStep), step);
        if (!isInRange)
            return null;

        return step;
    }
}
