using MyTelegram.Schema.Langpack.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Langpack;

internal sealed class GetLanguagesConverter
    : IRequestConverter<
        RequestGetLanguages,
        Schema.Langpack.RequestGetLanguages
    >, ITransientDependency
{
    public Schema.Langpack.RequestGetLanguages ToLatestLayerData(IRequestInput request, RequestGetLanguages obj)
    {
        return new Schema.Langpack.RequestGetLanguages
        {
            LangPack = request.DeviceType.ToString().ToLower()
        };
    }
}