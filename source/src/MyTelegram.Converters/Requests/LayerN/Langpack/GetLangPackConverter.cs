using MyTelegram.Schema.Langpack.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Langpack;

internal sealed class GetLangPackConverter
    : IRequestConverter<
        RequestGetLangPack,
        Schema.Langpack.RequestGetLangPack
    >, ITransientDependency
{
    public Schema.Langpack.RequestGetLangPack ToLatestLayerData(IRequestInput request, RequestGetLangPack obj)
    {
        return new Schema.Langpack.RequestGetLangPack
        {
            LangCode = obj.LangCode,
            LangPack = request.DeviceType.ToString().ToLower()
        };
    }
}