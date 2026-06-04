namespace MyTelegram.ReadModel.Interfaces;

/// <summary>
/// ReadModel для группировки кастомных эмодзи по обычному emoji
/// Соответствует TL-конструктору stickerPack
/// </summary>
public interface IEmojiPackReadModel : IReadModel
{
    /// <summary>
    /// ID стикерсета
    /// </summary>
    long StickerSetId { get; }

    /// <summary>
    /// Обычный emoji (UTF-8), к которому привязаны кастомные эмодзи
    /// </summary>
    string Emoticon { get; }

    /// <summary>
    /// Список document_id кастомных эмодзи, соответствующих этому emoji
    /// </summary>
    List<long> DocumentIds { get; }
}
