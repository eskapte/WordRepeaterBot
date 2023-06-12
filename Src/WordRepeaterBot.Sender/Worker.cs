using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using WordRepeaterBot.DataAccess;
using WordRepeaterBot.DataAccess.Models;

namespace WordRepeaterBot.Sender;

internal class Worker : BackgroundService
{
    private static readonly Dictionary<RepeatFrequencyInDay, byte[]> _shedules = new()
    {
        [RepeatFrequencyInDay.One] = new byte[] { 16 },
        [RepeatFrequencyInDay.Two] = new byte[] { 14, 18 },
        [RepeatFrequencyInDay.Three] = new byte[] { 13, 16, 19 },
        [RepeatFrequencyInDay.Four] = new byte[] { 10, 13, 16, 19 },
        [RepeatFrequencyInDay.Six] = new byte[] { 10, 12, 14, 16, 18, 20 },
        [RepeatFrequencyInDay.Tvelwe] = new byte[]
        {
            10, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 20, 21
        }
    };

    private const string LEARNING_TEMPLATE = "{0} \\- {1}";
    private const string REPEATER_TEMPLATE = "{0} \\- ||{1}||";

    private readonly WordRepeaterBotDbContext _dbContext;
    private readonly ITelegramBotClient _botClient;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<Worker> _logger;

    public Worker(
        WordRepeaterBotDbContext dbContext, 
        ITelegramBotClient botClient, 
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<Worker> logger)
    {
        _dbContext = dbContext;
        _botClient = botClient;
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        _logger.LogInformation("{0}: start sender job", DateTime.Now);

        try
        {
            var messages = await GetUsersIdsToSendAsync(token);

            foreach (var message in messages)
            {
                var text = message.Phrase.State == PhraseState.Learning ? LEARNING_TEMPLATE : REPEATER_TEMPLATE;
                var inlineText = message.Phrase.State == PhraseState.Learning ? "На повтор" : "Выучено";
                var newState = ++message.Phrase.State;

                var inlineButton = InlineKeyboardButton.WithCallbackData(
                    inlineText,
                    $"{message.Phrase.Id} {(byte)newState}");
                var inlineKeyboard = new InlineKeyboardMarkup(inlineButton);

                await _botClient.SendTextMessageAsync(
                    message.ChatId,
                    string.Format(text, message.Phrase.Text, message.Phrase.Translation),
                    Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
                    replyMarkup: inlineKeyboard,
                    cancellationToken: token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "{ExMessage} ${InnerExceptionMessage}\\n ${ExStackTrace}", 
                ex.Message, ex.InnerException?.Message, ex.StackTrace);
        }
        finally
        {
            _logger.LogInformation("{Date}: end sender job", DateTime.Now);
            _hostApplicationLifetime.StopApplication();
        }
    }

    private record UserPhrase(long UserId, long ChatId, Phrase Phrase);
    private async Task<IEnumerable<UserPhrase>> GetUsersIdsToSendAsync(CancellationToken token = default)
    {
        var usersPhrase = new List<UserPhrase>();
        var random = new Random();

        foreach (var shedule in _shedules)
        {
            var frequency = shedule.Key;
            var hours = shedule.Value.ToList();
            var utcHour = DateTime.UtcNow.Hour;

            // More chance for send phrase in state "Repeating" than "Learning".
            var state = random.NextSingle() > 0.7f ? PhraseState.Repeating : PhraseState.Learning;

            var userPhrasesQuery =
                from user in _dbContext.Users.Include(x => x.Phrases)
                join settings in _dbContext.Settings on user.UserId equals settings.UserId

                let phrases = user.Phrases.ToArray()
                let phrasesCount = phrases.Count()
                let isAnyRepeating = phrases.Any(x => x.State == PhraseState.Repeating)
                let filteredPhrases = isAnyRepeating
                    ? phrases.Where(x => x.State == state).ToArray()
                    : phrases

                where !user.IsDisabled 
                      && settings.FrequencePerDay == frequency
                      && hours.Contains((byte)(utcHour + settings.TimeZoneOffset))
                      && phrasesCount > 0
                select new UserPhrase(user.UserId, user.ChatId, filteredPhrases[random.Next(phrasesCount)]);

            var userPhrases = await userPhrasesQuery.ToListAsync(token);

            usersPhrase.AddRange(userPhrases);
        }

        return usersPhrase;
    }
}
