namespace WordRepeaterBot.Static;

public class BotCommand : IEquatable<string>
{
    private readonly string _command;
    protected BotCommand(string command) => _command = command;

    public readonly static BotCommand Start = new("/start");

    public bool Equals(string other) => string.Equals(_command, other, StringComparison.InvariantCultureIgnoreCase);
    public override bool Equals(object obj)
    {
        if (obj is null)
            return false;

        return Equals(obj as string);
    }

    public static bool operator ==(BotCommand left, string right) => left.Equals(right);

    public static bool operator ==(string left, BotCommand right) => right.Equals(left);

    public static bool operator !=(BotCommand left, string right) => !(left == right);

    public static bool operator !=(string left, BotCommand right) => !(left == right);

    public override int GetHashCode() => string.GetHashCode(_command);
}
