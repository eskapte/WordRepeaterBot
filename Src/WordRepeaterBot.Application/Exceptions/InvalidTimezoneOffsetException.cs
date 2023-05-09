namespace WordRepeaterBot.Application.Exceptions;

public class InvalidTimezoneOffsetException : WithUserResponseWithUserResponse
{
    public InvalidTimezoneOffsetException(long chatId) : base(chatId, "Такого часового пояса не существует. " +
        "Укажите число в промежутке от -12 до 14")
    { }

    public static void ThrowIfInvalid(int timezoneOffset, long chatId)
    {
        if (timezoneOffset < -12 || timezoneOffset > 14)
        {
            throw new InvalidTimezoneOffsetException(chatId);
        }
    }
}
