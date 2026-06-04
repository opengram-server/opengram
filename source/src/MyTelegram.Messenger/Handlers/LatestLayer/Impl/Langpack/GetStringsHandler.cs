namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Langpack;

///<summary>
/// Get strings from a language pack
/// <para>Possible errors</para>
/// Code Type Description
/// 400 LANG_CODE_NOT_SUPPORTED The specified language code is not supported.
/// 400 LANG_PACK_INVALID The provided language pack is invalid.
/// See <a href="https://corefork.telegram.org/method/langpack.getStrings" />
///</summary>
internal sealed class GetStringsHandler(ILanguageCacheService languageCacheService) : RpcResultObjectHandler<MyTelegram.Schema.Langpack.RequestGetStrings, TVector<MyTelegram.Schema.ILangPackString>>,
    Langpack.IGetStringsHandler
{
    protected override async Task<TVector<ILangPackString>> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Langpack.RequestGetStrings obj)
    {
        var texts = (await languageCacheService.GetLanguageTextsAsync(obj.LangCode, obj.LangPack, obj.Keys)).ToDictionary(k => k.Key, v => v);
        var langPackStrings = new TVector<ILangPackString>();

        foreach (var key in obj.Keys)
        {
            if (texts.TryGetValue(key, out var item))
            {
                langPackStrings.Add(new TLangPackString
                {
                    Key = item.Key,
                    Value = item.Value
                });
            }
            else
            {
                langPackStrings.Add(new TLangPackStringDeleted
                {
                    Key = key
                });
            }
        }

        return langPackStrings;
    }
}
