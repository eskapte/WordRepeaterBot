using Microsoft.EntityFrameworkCore;
using WordRepeaterBot.Application.Exceptions;
using WordRepeaterBot.DataAccess;
using WordRepeaterBot.DataAccess.Models;

namespace WordRepeaterBot.Application.Services;

public interface IPhraseService
{
    Task<Phrase> ParsePhraseAsync(long userId, string text, CancellationToken token = default);
    Task UpdatePhraseStateAsync(long phraseId, PhraseState newState, CancellationToken token = default);
}

public class PhraseService : IPhraseService
{
    private const string DELIMITER = ":";

    private readonly WordRepeaterBotDbContext _dbContext;

    public PhraseService(WordRepeaterBotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Phrase> ParsePhraseAsync(long userId, string text, CancellationToken token = default)
    {
        var phrases = text.Split(DELIMITER, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

        if (phrases.Count() != 2)
        {
            var user = await _dbContext.Users.AsNoTracking().FirstAsync(x => x.UserId == userId, token);
            throw new PhraseParseException(user.ChatId, "Пожалуйста, укажите сообщение по шаблону *Фраза : Перевод*");
        }

        var phrase = new Phrase
        {
            UserId = userId,
            Text = phrases[0],
            Translation = phrases[1],
            State = PhraseState.Learning
        };

        await _dbContext.AddAsync(phrase, token);
        await _dbContext.SaveChangesAsync(token);

        return phrase;
    }

    public async Task UpdatePhraseStateAsync(long phraseId, PhraseState newState, CancellationToken token = default)
    {
        var phrase = await _dbContext.Phrases.FirstAsync(x => x.Id == phraseId, token);

        if (phrase.State == newState)
            return;

        phrase.State = newState;

        await _dbContext.SaveChangesAsync(token);
    }
}
