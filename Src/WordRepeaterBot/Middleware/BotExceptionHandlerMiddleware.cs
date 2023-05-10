using System.Net;
using Telegram.Bot;
using WordRepeaterBot.Application.Exceptions;

namespace WordRepeaterBot.Middleware;

public class BotExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ILogger<BotExceptionHandlerMiddleware> _logger;

    public BotExceptionHandlerMiddleware(
        RequestDelegate next, 
        ITelegramBotClient telegramBotClient, 
        ILogger<BotExceptionHandlerMiddleware> logger = null)
    {
        _next = next;
        _telegramBotClient = telegramBotClient;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (WithUserResponseWithUserResponse ex)
        {
            await _telegramBotClient.SendTextMessageAsync(
                ex.ChatId,
                $"Ошибка: {ex.Message}");
        }
        catch(Exception ex)
        {
            _logger.LogWarning(ex.Message + " " + ex.InnerException?.Message + " " + ex.StackTrace);
        }

        context.Response.StatusCode = (int)HttpStatusCode.OK;
    }
}
