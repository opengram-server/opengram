using MyTelegram.Schema.Langpack.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Langpack;

internal sealed class GetStringsConverter
    : IRequestConverter<
        RequestGetStrings,
        Schema.Langpack.RequestGetStrings
    >, ITransientDependency
{
    public Schema.Langpack.RequestGetStrings ToLatestLayerData(IRequestInput request, RequestGetStrings obj)
    {
        return new Schema.Langpack.RequestGetStrings
        {
            LangCode = obj.LangCode,
            Keys = obj.Keys,
            LangPack = request.DeviceType.ToString().ToLower()
        };
    }
}