using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WordRepeaterBot.Application.Services;
using WordRepeaterBot.DataAccess.Models;
using WordRepeaterBot.Models;

namespace WordRepeaterBot.Services;

public interface IResponseService
{
    Task<ResponseMessages> GetResponseAsync(Update update, CancellationToken token = default);
}

public class ResponseService : IResponseService
{
    private readonly ISettingsService _settingsService;
    private readonly IUserService _userService;
    private readonly IPhraseService _phraseService;

    public ResponseService(ISettingsService settingsService, IUserService userService, IPhraseService phraseService)
    {
        _settingsService = settingsService;
        _userService = userService;
        _phraseService = phraseService;
    }

    // Добавить возможность просмотра пользователем статистики
    // Проверить, есть ли эвент на блокировку от пользователя
    public async Task<ResponseMessages> GetResponseAsync(Update update, CancellationToken token = default)
    {
        var responses = update.Type switch
        {
            UpdateType.Message => await OnMessageAsync(update.Message, token),
            UpdateType.CallbackQuery => await OnCallbackQueryAsync(update.CallbackQuery, token),
            _ => ResponseMessages.Empty()
        };

        return responses;
    }

    private async Task<ResponseMessages> OnMessageAsync(Message message, CancellationToken token = default)
    {
        var userId = message.From.Id;

        if (IsBotCommand(message, Static.BotCommand.Start))
        {
            return await InitUserAsync(message, token);
        }

        var settingsStep = await _settingsService.GetSettingsSetupStepAsync(userId, token);
        if (settingsStep.HasValue)
        {
            return await SetupUserAsync(message, settingsStep.Value, token);
        }

        var newPhrase = await _phraseService.ParsePhraseAsync(userId, message.Text, token);

        if (newPhrase != null)
        {
            return new ResponseMessages(Static.ResponseTexts.PhraseAddedSuccessful);
        }
        else
        {
            return new ResponseMessages(Static.ResponseTexts.PhraseAddedFailed);
        }
    }

    private async Task<ResponseMessages> OnCallbackQueryAsync(CallbackQuery callback, CancellationToken token = default)
    {
        var payload = callback.Data.Split(" ");
        var phraseId = long.Parse(payload[0]);
        var phraseNewState = (PhraseState)byte.Parse(payload[1]);

        await _phraseService.UpdatePhraseStateAsync(phraseId, phraseNewState, token);

        var responseText = phraseNewState == PhraseState.Repeating
            ? Static.ResponseTexts.OnRepeat
            : Static.ResponseTexts.OnLearned;

        return new(responseText);
    }

    private async Task<ResponseMessages> InitUserAsync(Message message, CancellationToken token = default)
    {
        var userId = message.From.Id;
        var isUserExist = await _userService.IsUserExistAsync(userId, token);

        if (isUserExist)
        {
            await _userService.EnableUserAsync(userId, token);
        }
        else
        {
            await _userService.CreateUserAsync(userId, message.Chat.Id, message.From.Username, token);
        }

        await _settingsService.SetSettingsSetupStepAsync(userId, SettingsSetupStep.SetTimeZone, token);

        return new ResponseMessages(Static.ResponseTexts.Start, Static.ResponseTexts.SetupTimeZone);
    }

    private async Task<ResponseMessages> SetupUserAsync(
        Message message, 
        SettingsSetupStep? step, 
        CancellationToken token = default)
    {
        var userId = message.From.Id;
        if (short.TryParse(message.Text, out var value))
        {
            await _settingsService.SetSettingsValue(userId, step.Value, value, token);
            step = step.NextStep();
            await _settingsService.SetSettingsSetupStepAsync(userId, step, token);
        }
        else
        {
            return new ResponseMessages(Static.ResponseTexts.InvilidSetupValue);
        }

        return new(GetNextMessageByStep(step));
    }

    private bool IsBotCommand(Message message, Static.BotCommand botCommand)
    {
        var isBotCommand = message.Entities != null && message.Entities.Any(x => x.Type == MessageEntityType.BotCommand);

        return isBotCommand && message.Text == botCommand;
    }

    private ResponseMessage GetNextMessageByStep(SettingsSetupStep? step) => step switch
    {
        SettingsSetupStep.SetTimeZone => new(Static.ResponseTexts.SetupTimeZone),
        SettingsSetupStep.SetFrequence => new(Static.ResponseTexts.SetupFrequence, Static.Keyboards.SetupFrequence),
        null => new(Static.ResponseTexts.SetupCompleted, Static.Keyboards.Empty),
        _ => throw new NotImplementedException()
    };
}
