namespace MyTelegram.ReadModel.Interfaces;

/// <summary>
/// ReadModel для категорий эмодзи
/// Соответствует TL-конструктору messages.emojiGroups
/// </summary>
public interface IEmojiGroupReadModel : IReadModel
{
    /// <summary>
    /// Тип категорий: "emoji_groups", "sticker_groups", "status_groups", "profile_groups"
    /// </summary>
    string CategoryType { get; }

    /// <summary>
    /// Список групп эмодзи
    /// </summary>
    List<EmojiGroup> Groups { get; }

    /// <summary>
    /// Hash для кэширования
    /// </summary>
    long Hash { get; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    DateTime UpdatedAt { get; }
}

/// <summary>
/// Группа эмодзи (категория)
/// </summary>
public class EmojiGroup
{
    /// <summary>
    /// Название категории (например, "Animals", "Faces")
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// ID кастомного эмодзи для иконки категории
    /// </summary>
    public long IconEmojiId { get; set; }

    /// <summary>
    /// Список обычных emoji, относящихся к этой категории
    /// </summary>
    public List<string> Emoticons { get; set; } = new();

    /// <summary>
    /// Является ли группа Premium-категорией
    /// </summary>
    public bool IsPremium { get; set; }

    /// <summary>
    /// Тип группы: "regular", "greeting", "premium"
    /// </summary>
    public string GroupType { get; set; } = "regular";
}
