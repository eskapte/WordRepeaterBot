using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using WordRepeaterBot.Services;
using Telegram.Bot.Types.Enums;
using WordRepeaterBot.Application.Services;
using WordRepeaterBot.DataAccess.Models;

namespace WordRepeaterBot.Controllers;

[Route("bot")]
public class UpdateController : Controller
{
    private readonly IResponseService _responseService;
    private readonly ILogger<UpdateController> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IPhraseService _phraseService;

    public UpdateController(
        IResponseService responseService,
        ILogger<UpdateController> logger,
        ITelegramBotClient botClient,
        IPhraseService phraseService)
    {
        _responseService = responseService;
        _logger = logger;
        _botClient = botClient;
        _phraseService = phraseService;
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
            await OnCallbackQueryAsync(update, token);
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

    private async Task OnCallbackQueryAsync(Update update, CancellationToken token = default)
    {
        var callbackQuery = update.CallbackQuery;
        if (callbackQuery is null)
        {
            _logger.LogError($"{nameof(callbackQuery)} is null");
            return;
        }

        var payload = callbackQuery.Data.Split(" ");
        var phraseId = long.Parse(payload[0]);
        var phraseNewState = (PhraseState)byte.Parse(payload[1]);

        await _phraseService.UpdatePhraseStateAsync(phraseId, phraseNewState, token);

        // TODO: статистика выученных слов
        var responseText = phraseNewState == PhraseState.Repeating 
            ? "Фраза добавлена на повторение" 
            : "Отлично, ещё одно выученное слово";

        await _botClient.SendTextMessageAsync(
            callbackQuery.Message.Chat.Id,
            responseText,
            ParseMode.MarkdownV2,
            cancellationToken: token);

        // TODO: обновлять инлайн кнопки потом
    }
}
