namespace WordRepeaterBot.Static;

public static class ResponseTexts
{
    public readonly static string Start =
        "Добро пожаловать\\!\r\n\r\n" +
        "Этот бот помогает учить и повторять иностранные слова\r\n" +
        "Отправляйте сообщения в виде *Фраза/слово : перевод*, к примеру:\r\n\r\n" +
        "*Good morning : Доброе утро* \\(Порядок и двоеточие важны\\)\r\n\r\n" +
        "Бот будет запоминать их и отправлять вам с некой периодичностью, которую вы сможете настроить далее\r\n\r\n";

    public readonly static string SetupTimeZone = "Укажите ваше смещение часового пояса числом " +
        "\\(К примеру, для Москвы это *3*\\)\r\n" +
        "Это нужно для того, чтобы бот не писал вам ночью \\(с 21:00 до 10:00\\)";

    public readonly static string InvilidSetupValue = "Заданное значение имеет неверный формат";

    public readonly static string SetupFrequence = "Укажите через клавиатуру бота, сколько слов в день вам присылать\r\n" +
        "Валидные значения: 1, 2, 3, 4, 6 и 12";

    public readonly static string SetupCompleted = "Настройка завершена\\! Вы можете изменить настройки в любой момент, " +
        "нажав на кнопку *Настройки* в меню бота\r\n\r\n" +
        "Удачи в изучении новых слов\\!";

    public readonly static string PhraseAddedSuccessful = "Фраза добавлена";
    public readonly static string PhraseAddedFailed = "Не удалось добавить новую фразу. Попробуйте ещё раз.";

    public readonly static string UnknowCommand = "Неизвестная команда";
}
