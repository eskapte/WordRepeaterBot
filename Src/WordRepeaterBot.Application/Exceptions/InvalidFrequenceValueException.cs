namespace WordRepeaterBot.Application.Exceptions;

public class InvalidFrequenceValueException : WithUserResponseWithUserResponse
{
    public InvalidFrequenceValueException(long chatId) : base(chatId, "Неправильное значение")
    {

    }
}
