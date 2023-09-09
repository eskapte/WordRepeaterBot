namespace WordRepeaterBot.Static;

public static class ResponseTexts
{
    public static readonly string Start =
        "Добро пожаловать\\!\r\n\r\n" +
        "Этот бот помогает учить и повторять иностранные слова\r\n" +
        "Отправляйте сообщения в виде *Фраза/слово : перевод*, к примеру:\r\n\r\n" +
        "*Good morning : Доброе утро* \\(Порядок и двоеточие важны\\)\r\n\r\n" +
        "Бот будет запоминать их и отправлять вам с некой периодичностью, которую вы сможете настроить далее\r\n\r\n";

    public static readonly string UserReturn =
        "С возвращением!";

    public static readonly string SetupTimeZone = "Укажите ваше смещение часового пояса числом " +
                                                  "\\(К примеру, для Москвы это *3*\\)\r\n" +
                                                  "Это нужно для того, чтобы бот не писал вам ночью \\(с 21:00 до 10:00\\)";

    public static readonly string InvalidSetupValue = "Заданное значение имеет неверный формат";

    public static readonly string SetupFrequence = "Укажите через клавиатуру бота, сколько слов в день вам присылать\r\n" +
        "Валидные значения: 1, 2, 3, 4, 6 и 12";

    public static readonly string SetupCompleted = "Настройка завершена\\! Вы можете изменить настройки в любой момент, " +
        "нажав на кнопку *Настройки* в меню бота\r\n\r\n" +
        "Удачи в изучении новых слов\\!";

    public static readonly string PhraseAddedSuccessful = "Фраза добавлена";
    public static readonly string PhraseAddedFailed = "Не удалось добавить новую фразу. Попробуйте ещё раз.";

    public static readonly string Statistics = "Слов изучается: {0}\n" +
                                               "Слов на повторении: {1}\n" +
                                               "Слов изучено: {2}";
    public static readonly string OnRepeat = $"Фраза добавлена на повторение\n\n{Statistics}";
    public static readonly string OnLearned = $"Отлично, ещё одна выученная фраза\n\n{Statistics}";

    public static readonly string UnknownCommand = "Неизвестная команда";
}
