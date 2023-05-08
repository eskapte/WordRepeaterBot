namespace WordRepeaterBot.DataAccess.Models;

/// <summary>
/// Пользователь телеграмма с необходимыми данными для отправки сообщений ему
/// </summary>
public class User
{
    /// <summary>
    /// Соответствует Id в Телеграмме
    /// </summary>
    public long UserId { get; set; }
    public long ChatId { get; set; }
    public string? Username { get; set; }
    /// <summary>
    /// Пользователь может удалить диалог и запретить боту присылать сообщения
    /// </summary>
    public bool IsDisabled { get; set; }
    public DateTime Created { get; set; }

    public List<Phrase> Phrases { get; set; } = new();
    public Settings? Settings { get; set; }
}
