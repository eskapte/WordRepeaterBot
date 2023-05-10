using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using WordRepeaterBot.DataAccess;
using WordRepeaterBot.Sender;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;
        var botToken = config.GetValue<string>("BotToken");
        ArgumentException.ThrowIfNullOrEmpty(botToken);

        services.AddDbContext<WordRepeaterBotDbContext>(builder =>
        {
            var connectionString = config.GetConnectionString("WordRepeaterBotDb");
            builder.UseNpgsql(connectionString);
        })
        .AddHostedService<Worker>()
        .AddHttpClient("telegram_bot_client")
        .AddTypedClient<ITelegramBotClient>((httpClient) =>
        {
            var botOptions = new TelegramBotClientOptions(botToken);
            return new TelegramBotClient(botOptions, httpClient);
        });
    })
    .Build()
    .RunAsync();
