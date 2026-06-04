namespace MyTelegram.ReadModel.Interfaces;

public interface IChatThemeReadModel : IReadModel
{
    long UserId { get; }

    /// <summary>
    ///     UserId/ChatId/ChannelId
    /// </summary>
    long ChatId { get; }

    long ThemeId { get; }
    string Emoticon { get; }
}