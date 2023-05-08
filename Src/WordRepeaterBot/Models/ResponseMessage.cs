using Telegram.Bot.Types.ReplyMarkups;

namespace WordRepeaterBot.Models;

public record ResponseMessage(string Text, IReplyMarkup Markup = null);

public class ResponseMessages : List<ResponseMessage>
{
    public ResponseMessages(params ResponseMessage[] responseMessages) => AddRange(responseMessages);

    public ResponseMessages(params string[] responseMessages)
        => AddRange(responseMessages.Select(x => new ResponseMessage(x, null)));

    public static ResponseMessages Empty()
    {
        return new ResponseMessages(Enumerable.Empty<ResponseMessage>().ToArray());
    }
}
