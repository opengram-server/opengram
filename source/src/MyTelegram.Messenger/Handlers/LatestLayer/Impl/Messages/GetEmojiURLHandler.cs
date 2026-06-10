namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Returns an HTTP URL which can be used to automatically log in and get the emoji keywords.
/// See <a href="https://corefork.telegram.org/method/messages.getEmojiURL" />
///</summary>
internal sealed class GetEmojiURLHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetEmojiURL, MyTelegram.Schema.IEmojiURL>,
    Messages.IGetEmojiURLHandler
{
    protected override Task<IEmojiURL> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetEmojiURL obj)
    {
        // Return an emoji keywords URL. On self-hosted, point to the standard endpoint.
        var langCode = obj.LangCode ?? "en";

        return Task.FromResult<IEmojiURL>(new TEmojiURL
        {
            Url = $"https://emojik.ru/klyuchevye-slova/{langCode}/"
        });
    }
}
