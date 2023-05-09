namespace WordRepeaterBot.Application.Exceptions;

public abstract class WithUserResponseWithUserResponse : Exception
{
    public long ChatId { get; }
    public WithUserResponseWithUserResponse(long chatId, string? message) : base(message)
    {
        ChatId = chatId;
    }
}
