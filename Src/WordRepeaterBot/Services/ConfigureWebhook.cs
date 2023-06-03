using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using WordRepeaterBot.Configuration;

namespace WordRepeaterBot.Services;

public class ConfigureWebhook : IHostedService
{
    private readonly BotConfig _botConfig;
    private readonly ITelegramBotClient _telegramBotClient;

    public ConfigureWebhook(IOptions<BotConfig> botConfig, ITelegramBotClient telegramBotСlient)
    {
        _botConfig = botConfig.Value;
        _telegramBotClient = telegramBotСlient;
    }

    public async Task StartAsync(CancellationToken token = default)
    {
        var absolutePath = $"{_botConfig.Host}/{_botConfig.UpdateRoute}";
        await _telegramBotClient.SetWebhookAsync(
            url: absolutePath,
            allowedUpdates: Array.Empty<UpdateType>(),
            cancellationToken: token);
    }

    public async Task StopAsync(CancellationToken token = default)
    {
        await _telegramBotClient.DeleteWebhookAsync(cancellationToken: token);
    }
}
