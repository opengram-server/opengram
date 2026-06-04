// ReSharper disable All

namespace MyTelegram.Converters.TLObjects;

internal sealed class StarGiftConverter : IStarGiftConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;
}
