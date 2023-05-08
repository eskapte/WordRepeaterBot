using System.ComponentModel.DataAnnotations.Schema;

namespace WordRepeaterBot.DataAccess.Models;

/// <summary>
/// Фразы с переводом
/// </summary>
public class Phrase
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Translation { get; set; } = string.Empty;
    public PhraseState State { get; set; }
}

public enum PhraseState : byte
{
    Learning = 1,
    Repeating,
    Learned
}
