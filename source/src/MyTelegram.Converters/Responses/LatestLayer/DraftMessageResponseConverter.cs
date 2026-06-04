// ReSharper disable All

namespace MyTelegram.Converters.Responses;

internal sealed class DraftMessageResponseConverter : IDraftMessageResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public Schema.IDraftMessage ToLayeredData(Schema.TDraftMessage obj)
    {
        return obj;
    }
}