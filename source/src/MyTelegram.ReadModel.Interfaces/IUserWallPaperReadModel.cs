using MyTelegram.Schema;

namespace MyTelegram.ReadModel.Interfaces;

public interface IUserWallPaperReadModel : IReadModel
{
    long UserId { get; }
    long WallPaperId { get; }
    bool IsSaved { get; }
    bool IsInstalled { get; }
    WallPaperSettings? WallPaperSettings { get; }
}