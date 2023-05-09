using Microsoft.EntityFrameworkCore;
using WordRepeaterBot.Application.Exceptions;
using WordRepeaterBot.DataAccess;
using WordRepeaterBot.DataAccess.Models;

namespace WordRepeaterBot.Application.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(long userId, long chatId, string userName, CancellationToken token = default);
    Task<bool> IsUserExistAsync(long userId, CancellationToken token = default);
    Task EnableUserAsync(long userId, CancellationToken token = default);
}

public class UserService : IUserService
{
    private readonly WordRepeaterBotDbContext _dbContext;

    public UserService(WordRepeaterBotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> CreateUserAsync(long userId, long chatId, string? username, CancellationToken token = default)
    {
        var isUserExist = await _dbContext.Users.AnyAsync(x => x.UserId == userId, token);
        if (isUserExist)
            throw new UserExistException(userId);

        var settings = new Settings
        {
            UserId = userId,
            FrequencePerDay = null,
            SettingsSetupStep = null,
            TimeZoneOffset = 0
        };
        var user = new User
        {
            ChatId = chatId,
            UserId = userId,
            Username = username,
            Created = DateTime.UtcNow,
            IsDisabled = false,
            Settings = settings
        };

        await _dbContext.Users.AddAsync(user, token);
        await _dbContext.SaveChangesAsync(token);

        return user;
    }

    public async Task EnableUserAsync(long userId, CancellationToken token = default)
    {
        var user = await _dbContext.Users.SingleAsync(x => x.UserId == userId, token);
        user.IsDisabled = false;
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task<bool> IsUserExistAsync(long userId, CancellationToken token = default)
    {
        return await _dbContext.Users.AnyAsync(x => x.UserId == userId, token);
    }
}
