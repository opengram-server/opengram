namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Mark mentions as read.
/// See <a href="https://corefork.telegram.org/method/messages.readMentions" />
///</summary>
internal sealed class ReadMentionsHandler(
    IPtsHelper ptsHelper,
    IPeerHelper peerHelper,
    IAccessHashHelper accessHashHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestReadMentions, MyTelegram.Schema.Messages.IAffectedHistory>,
    Messages.IReadMentionsHandler
{
    protected override async Task<MyTelegram.Schema.Messages.IAffectedHistory> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestReadMentions obj)
    {
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);
        await accessHashHelper.CheckAccessHashAsync(input, obj.Peer);

        // Use the user's (or channel's) current PTS
        var pts = ptsHelper.GetCachedPts(peer.PeerId);

        // Offset=0 means all mentions have been marked as read (no more chunks)
        return new MyTelegram.Schema.Messages.TAffectedHistory
        {
            Pts = pts,
            PtsCount = 0,
            Offset = 0
        };
    }
}
