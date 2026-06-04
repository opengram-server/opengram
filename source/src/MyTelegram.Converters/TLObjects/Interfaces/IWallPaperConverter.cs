using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IWallPaperConverter : ILayeredConverter
{
    IWallPaper ToWallPaper(long selfUserId, IWallpaperReadModel wallPaperReadModel, IDocument? document);
    IWallPaper ToWallPaper(long selfUserId, WallPaper wallPaper, IDocument? document);

    List<IWallPaper> ToWallPapers(long selfUserId, IReadOnlyCollection<IWallpaperReadModel> wallPaperReadModels,
        IReadOnlyCollection<IDocument> documents);
}