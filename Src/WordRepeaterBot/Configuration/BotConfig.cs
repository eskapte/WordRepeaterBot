namespace WordRepeaterBot.Configuration;

public class BotConfig
{
    public const string SECTION = nameof(BotConfig);
    public string Host { get; set; }
    public string UpdateRoute { get; set; }
    public string BotToken { get; set; }
}
