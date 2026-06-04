namespace MyTelegram.ReadModel;

public class ChatWallPaper : IChatWallPaperReadModel
{
    public string Id { get; set; } = default!;
    public long UserId { get; set; }
    public long ChatId { get; set; }
    public long WallPaperId { get; set; }
    public bool ForBoth { get; set; }
    public WallPaperSettings? WallPaperSettings { get; set; }
}
