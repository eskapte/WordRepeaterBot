using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using WordRepeaterBot.Services;
using Telegram.Bot.Types.Enums;

namespace WordRepeaterBot.Controllers;

[Route("bot")]
public class UpdateController : Controller
{
    private readonly IResponseService _responseService;
    private readonly ILogger<UpdateController> _logger;
    private readonly ITelegramBotClient _botClient;

    public UpdateController(
        IResponseService responseService, 
        ILogger<UpdateController> logger, 
        ITelegramBotClient botClient)
    {
        _responseService = responseService;
        _logger = logger;
        _botClient = botClient;
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody]Update update, CancellationToken token = default)
    {
        if (update is null)
        {
            _logger.LogWarning($"{nameof(update)} is null");
            return BadRequest();
        }


        if (update.Type == UpdateType.CallbackQuery)
        {
            return Ok();
        }

        if (update.Type != UpdateType.Message)
            return Ok();

        var message = update.Message;

        var responses = await _responseService.GetResponseAsync(message, token);

        foreach (var response in responses)
        {
            if (response is null || string.IsNullOrWhiteSpace(response.Text)) 
                continue;
            await ResponseAsync(response, update, token);
        }

        return Ok();
    }

    private async Task ResponseAsync(Models.ResponseMessage response, Update update, CancellationToken token = default)
    {
        var msg = await _botClient.SendTextMessageAsync(
            update.Message.Chat.Id,
            response.Text,
            ParseMode.MarkdownV2,
            replyMarkup: response.Markup,
            cancellationToken: token);

        if (msg is null)
        {
            var userId = update.Message.From.Id;
            var chatId = update.Message.Chat.Id;
            _logger.LogError($"Failed to send message to user {userId} and chat {chatId}");
        }
    }
}
