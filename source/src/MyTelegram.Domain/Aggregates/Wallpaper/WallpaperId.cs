namespace MyTelegram.Domain.Aggregates.Wallpaper;

[JsonConverter(typeof(SingleValueObjectConverter))]
public class WallpaperId : Identity<WallpaperId>
{
    public WallpaperId(string value) : base(value)
    {
    }
}
