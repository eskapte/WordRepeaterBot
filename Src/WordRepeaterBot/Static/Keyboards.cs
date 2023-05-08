using Telegram.Bot.Types.ReplyMarkups;
using WordRepeaterBot.DataAccess.Models;

namespace WordRepeaterBot.Static;

public static class Keyboards
{
    public static IReplyMarkup SetupFrequence = new ReplyKeyboardMarkup(new KeyboardButton[][]
    {
        new KeyboardButton[]
        {
            new(((byte)RepeatFrequencyInDay.One).ToString()),
            new(((byte)RepeatFrequencyInDay.Two).ToString()),
        },
        new KeyboardButton[]
        {
            new(((byte) RepeatFrequencyInDay.Three).ToString()),
            new(((byte) RepeatFrequencyInDay.Four).ToString()),
        },
        new KeyboardButton[]
        {
            new(((byte) RepeatFrequencyInDay.Six).ToString()),
            new(((byte) RepeatFrequencyInDay.Tvelwe).ToString())
        }
    });

    public static IReplyMarkup Empty = new ReplyKeyboardRemove();
}
