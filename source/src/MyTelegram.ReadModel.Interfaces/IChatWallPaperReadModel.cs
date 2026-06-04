namespace MyTelegram.ReadModel.Interfaces;

public interface IChatWallPaperReadModel : IReadModel
{
    long UserId { get; }

    /// <summary>
    ///     UserId/ChatId/ChannelId
    /// </summary>
    long ChatId { get; }

    long WallPaperId { get; }
    bool ForBoth { get; }
    WallPaperSettings? WallPaperSettings { get; }
}