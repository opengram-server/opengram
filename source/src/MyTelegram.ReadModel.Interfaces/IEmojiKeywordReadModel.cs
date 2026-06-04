namespace MyTelegram.ReadModel.Interfaces;

/// <summary>
/// ReadModel для ключевых слов поиска кастомных эмодзи
/// Соответствует TL-конструктору stickerKeyword
/// </summary>
public interface IEmojiKeywordReadModel : IReadModel
{
    /// <summary>
    /// ID документа кастомного эмодзи
    /// </summary>
    long DocumentId { get; }

    /// <summary>
    /// Список ключевых слов для поиска (мультиязычные)
    /// </summary>
    List<string> Keywords { get; }
}
