using System.Diagnostics.CodeAnalysis;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class WallPaperMapper
    : IObjectMapper<IWallpaperReadModel, IWallPaper>,
        ILayeredMapper,
        ITransientDependency
{
    private readonly IObjectMapper _objectMapper;

    public WallPaperMapper(IObjectMapper objectMapper)
    {
        _objectMapper = objectMapper;
    }

    public int Layer => Layers.LayerLatest;

    public IWallPaper Map(IWallpaperReadModel source)
    {
        return Map(source, null!);
    }

    [return: NotNullIfNotNull("source")]
    public IWallPaper? Map(IWallpaperReadModel source, IWallPaper destination)
    {
        if (source == null)
        {
            return null;
        }

        // If wallpaper has a document, return TWallPaper
        if (source.DocumentId.HasValue)
        {
            var wallpaper = new TWallPaper
            {
                Id = source.WallpaperId,
                AccessHash = source.AccessHash,
                Slug = source.Slug,
                Default = source.Default,
                Pattern = source.Pattern,
                Dark = source.Dark,
                Creator = false, // Can be enhanced to check if creator
                Document = new TDocumentEmpty { Id = source.DocumentId.Value },
                Settings = source.Settings != null ? ConvertWallPaperSettings(source.Settings) : null
            };

            return wallpaper;
        }

        // Otherwise return TWallPaperNoFile
        return new TWallPaperNoFile
        {
            Id = source.WallpaperId,
            Default = source.Default,
            Dark = source.Dark,
            Settings = source.Settings != null ? ConvertWallPaperSettings(source.Settings) : null
        };
    }

    private static TWallPaperSettings ConvertWallPaperSettings(WallPaperSettings settings)
    {
        return new TWallPaperSettings
        {
            Blur = settings.Blur,
            Motion = settings.Motion,
            BackgroundColor = settings.BackgroundColor,
            SecondBackgroundColor = settings.SecondBackgroundColor,
            ThirdBackgroundColor = settings.ThirdBackgroundColor,
            FourthBackgroundColor = settings.FourthBackgroundColor,
            Intensity = settings.Intensity,
            Rotation = settings.Rotation,
            Emoticon = settings.Emoticon
        };
    }
}
