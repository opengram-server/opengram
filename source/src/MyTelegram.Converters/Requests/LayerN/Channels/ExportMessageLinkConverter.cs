using MyTelegram.Schema.Channels.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Channels;

internal sealed class ExportMessageLinkConverter
    : IRequestConverter<
        RequestExportMessageLink,
        Schema.Channels.RequestExportMessageLink
    >, ITransientDependency
{
    public Schema.Channels.RequestExportMessageLink ToLatestLayerData(IRequestInput request,
        RequestExportMessageLink obj)
    {
        return new Schema.Channels.RequestExportMessageLink
        {
            Channel = obj.Channel,
            Id = obj.Id
        };
    }
}