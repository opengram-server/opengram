namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Langpack;

///<summary>
/// Get new strings in language pack
/// <para>Possible errors</para>
/// Code Type Description
/// 400 LANG_PACK_INVALID The provided language pack is invalid.
/// See <a href="https://corefork.telegram.org/method/langpack.getDifference" />
///</summary>
internal sealed class GetDifferenceHandler(ILanguageCacheService languageCacheService) : RpcResultObjectHandler<MyTelegram.Schema.Langpack.RequestGetDifference, MyTelegram.Schema.ILangPackDifference>,
    Langpack.IGetDifferenceHandler
{
    protected override async Task<ILangPackDifference> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Langpack.RequestGetDifference obj)
    {
        var texts = await languageCacheService.GetLanguageDifferenceAsync(obj.LangCode, obj.LangPack, obj.FromVersion);

        var version = texts.FirstOrDefault()?.LanguageVersion ?? obj.FromVersion;

        var langPackDifference = new TLangPackDifference
        {
            FromVersion = obj.FromVersion,
            LangCode = obj.LangCode,
            Strings = [.. texts.Select(p => new TLangPackString
            {
                Key = p.Key,
                Value = p.Value
            })],
            Version = version
        };

        return langPackDifference;
    }
}
