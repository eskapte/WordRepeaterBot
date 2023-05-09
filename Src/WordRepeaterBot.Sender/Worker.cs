using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using WordRepeaterBot.DataAccess;
using WordRepeaterBot.DataAccess.Models;

namespace WordRepeaterBot.Sender;

internal class Worker : BackgroundService
{
    private readonly static Dictionary<RepeatFrequencyInDay, byte[]> _shedules = new()
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

    private const string LEARNING_TEMPLATE = "{0} - {1}";
    private const string REPEATER_TEMPLATE = "{0} - \\|\\|{1}\\|\\|";

    private static InlineKeyboardButton learningInline = new("Запомнил");
    private static InlineKeyboardButton repeatingInline = new("Выучено");

    private readonly WordRepeaterBotDbContext _dbContext;
    private readonly ITelegramBotClient _botClient;

    public Worker(WordRepeaterBotDbContext dbContext, ITelegramBotClient botClient)
    {
        _dbContext = dbContext;
        _botClient = botClient;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        var messages = await GetUsersIdsToSendAsync(token);

        foreach (var message in messages)
        {
            var text = message.Phrase.State == PhraseState.Learning ? LEARNING_TEMPLATE : REPEATER_TEMPLATE;
            var inline = message.Phrase.State == PhraseState.Learning ? learningInline : repeatingInline;
            var newState = message.Phrase.State++;

            inline = inline.CallbackData = $"{message.Phrase.Id} {newState}";
            var inlineKeyboard = new InlineKeyboardMarkup(inline!);

            await _botClient.SendTextMessageAsync(
                message.ChatId,
                string.Format(text, message.Phrase.Text, message.Phrase.Translation),
                Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
                replyMarkup: inlineKeyboard,
                cancellationToken: token);
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

            var userPhrasesQuery = 
                from user in _dbContext.Users
                join settings in _dbContext.Settings on user.UserId equals settings.UserId

                let phrasesCount = user.Phrases.Where(x => x.State != PhraseState.Learned).Count()
                let randomPhrase = user.Phrases.Where(x => x.State != PhraseState.Learned).ToArray()[random.Next(phrasesCount)]

                where !user.IsDisabled 
                    && settings.FrequencePerDay == frequency
                    && hours.Contains((byte)(utcHour + settings.TimeZoneOffset))
                    && phrasesCount > 0
                select new UserPhrase(user.UserId, user.ChatId, randomPhrase);

            var userPhrases = await userPhrasesQuery.Include("Phrases").ToListAsync(token);

            usersPhrase.AddRange(userPhrases);
        }

        return usersPhrase;
    }
}
