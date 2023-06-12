using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using WordRepeaterBot.Application.Extensions;
using WordRepeaterBot.Configuration;
using WordRepeaterBot.DataAccess;
using WordRepeaterBot.Middleware;
using WordRepeaterBot.Services;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

#region Services

var botConfigurationSection = config.GetSection(BotConfig.SECTION);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddHttpContextAccessor();

builder.Services
    .Configure<BotConfig>(botConfigurationSection)
    .AddHostedService<ConfigureWebhook>()
    .AddScoped<IResponseService, ResponseService>()
    .AddMemoryCache()
    .AddApplicationServices()
    .AddDbContext<WordRepeaterBotDbContext>(options =>
    {
        var connectionString = config.GetConnectionString("WordRepeaterBotDb");
        options.UseNpgsql(connectionString);
    })
    .AddHttpClient("telegram_bot_client")
    .AddTypedClient<ITelegramBotClient>((httpClient, serviceProvider) =>
    {
        var botConfig = serviceProvider.GetService<IOptions<BotConfig>>().Value;
        var botOptions = new TelegramBotClientOptions(botConfig.BotToken);
        return new TelegramBotClient(botOptions, httpClient);
    });

#endregion

var app = builder.Build();

#region Pipelines

app.UseMiddleware<BotExceptionHandlerMiddleware>();
app.MapControllers();

#endregion

app.Run();
