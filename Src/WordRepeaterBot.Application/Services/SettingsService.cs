using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WordRepeaterBot.Application.Exceptions;
using WordRepeaterBot.DataAccess;
using WordRepeaterBot.DataAccess.Models;

namespace WordRepeaterBot.Application.Services;

public interface ISettingsService
{
    Task SetSettingsSetupStepAsync(long userId, SettingsSetupStep? step, CancellationToken token = default);
    Task<SettingsSetupStep?> GetSettingsSetupStepAsync(long userId, CancellationToken token = default);
    Task SetSettingsValue(long userId, SettingsSetupStep step, short value, CancellationToken token = default);
}

public class SettingsService : ISettingsService
{
    private readonly WordRepeaterBotDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _memoryCacheOptions;
    private static readonly string _cacheKeyTemplate = "settings-setup-step-{0}";

    private long? _userId;

    public SettingsService(WordRepeaterBotDbContext dbContext, IMemoryCache memoryCache)
    {
        _dbContext = dbContext;
        _memoryCache = memoryCache;
        _memoryCacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromHours(1));
    }

    public async Task SetSettingsSetupStepAsync(long userId, SettingsSetupStep? step, CancellationToken token = default)
    {
        var userSettings = await _dbContext.Settings.FindAsync(new object[] { userId }, cancellationToken: token);
        SettingsNotFoundException.ThrowIfNull(userSettings, userId);

        userSettings!.SettingsSetupStep = step;

        var cacheKey = string.Format(_cacheKeyTemplate, userId);
        _memoryCache.Set(cacheKey, step, _memoryCacheOptions);

        await _dbContext.SaveChangesAsync(token);
    }

    public async Task<SettingsSetupStep?> GetSettingsSetupStepAsync(long userId, CancellationToken token = default)
    {
        var cacheKey = string.Format(_cacheKeyTemplate, userId);

        if (_memoryCache.TryGetValue(cacheKey, out SettingsSetupStep? settingsSetupStep))
        {
            return settingsSetupStep;
        }

        var userSettings = await _dbContext.Settings.FindAsync(new object[] { userId }, cancellationToken: token);
        SettingsNotFoundException.ThrowIfNull(userSettings, userId);

        _memoryCache.Set(cacheKey, userSettings!.SettingsSetupStep, _memoryCacheOptions);

        return userSettings!.SettingsSetupStep;
    }

    public async Task SetSettingsValue(long userId, SettingsSetupStep step, short value, CancellationToken token = default)
    {
        var user = await _dbContext.Users.FindAsync(new object[] { userId }, cancellationToken: token);
        var userSettings = await _dbContext.Settings.FindAsync(new object[] { userId }, cancellationToken: token);
        SettingsNotFoundException.ThrowIfNull(userSettings, userId);

        switch (step)
        {
            case SettingsSetupStep.SetTimeZone:
                InvalidTimezoneOffsetException.ThrowIfInvalid(value, user!.ChatId);
                userSettings!.TimeZoneOffset = value;
                break;
            case SettingsSetupStep.SetFrequence:
                if (!Enum.IsDefined(typeof(RepeatFrequencyInDay), (byte)value))
                {
                    throw new InvalidFrequenceValueException(user!.ChatId);
                }

                userSettings!.FrequencePerDay = (RepeatFrequencyInDay)value;
                break;
        }
    }

    private async Task<long> GetUserIdAsync(long chatId, CancellationToken token = default)
    {
        if (_userId.HasValue)
        {
            return _userId.Value;
        }

        var user = await _dbContext.Users.AsNoTracking().FirstAsync(x => x.ChatId == chatId, token);
        _userId = user.UserId;

        return _userId.Value;
    }
}
