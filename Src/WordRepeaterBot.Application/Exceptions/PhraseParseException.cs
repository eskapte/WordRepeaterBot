namespace WordRepeaterBot.Application.Exceptions;

public class PhraseParseException : WithUserResponseWithUserResponse
{

    public PhraseParseException(long chatId, string? message = null) : base(chatId, message)
    {
    }
}
