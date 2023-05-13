using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using WordRepeaterBot.Services;
using Telegram.Bot.Types.Enums;
using WordRepeaterBot.Application.Services;

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

        var responses = await _responseService.GetResponseAsync(update, token);

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
            GetChatId(update),
            response.Text,
            ParseMode.MarkdownV2,
            replyMarkup: response.Markup,
            cancellationToken: token);

        if (msg is null)
        {
            var userId = GetUserId(update);
            var chatId = GetChatId(update);
            _logger.LogError($"Failed to send message to user {userId} and chat {chatId}");
        }
    }

    private long GetUserId(Update update)
    {
        return update.Message?.From?.Id ?? update.CallbackQuery?.From.Id ?? 0;
    }

    private long GetChatId(Update update)
    {
        return update.Message?.Chat.Id ?? update.CallbackQuery?.Message.Chat.Id ?? 0;
    }
}
