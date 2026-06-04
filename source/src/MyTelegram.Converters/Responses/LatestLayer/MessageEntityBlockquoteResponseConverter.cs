// ReSharper disable All

namespace MyTelegram.Converters.Responses;

internal sealed class MessageEntityBlockquoteResponseConverter : IMessageEntityBlockquoteResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public Schema.IMessageEntity ToLayeredData(Schema.TMessageEntityBlockquote obj)
    {
        return obj;
    }
}