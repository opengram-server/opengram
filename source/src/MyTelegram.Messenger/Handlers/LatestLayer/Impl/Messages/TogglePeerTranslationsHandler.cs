namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Enable or disable real-time <a href="https://corefork.telegram.org/api/translation">chat translation for a certain chat</a>.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// See <a href="https://corefork.telegram.org/method/messages.togglePeerTranslations" />
///</summary>
internal sealed class TogglePeerTranslationsHandler(
    IAccessHashHelper accessHashHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestTogglePeerTranslations, IBool>,
        Messages.ITogglePeerTranslationsHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        RequestTogglePeerTranslations obj)
    {
        await accessHashHelper.CheckAccessHashAsync(input, obj.Peer);
        // Translation toggle is a client-side setting per the Telegram API.
        // The server acknowledges the toggle but doesn't enforce it.
        return new TBoolTrue();
    }
}
