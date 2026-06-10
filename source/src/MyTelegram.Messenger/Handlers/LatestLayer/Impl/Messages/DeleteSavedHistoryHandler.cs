namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Deletes saved messages history for the specified peer.
/// See <a href="https://corefork.telegram.org/method/messages.deleteSavedHistory" />
///</summary>
internal sealed class DeleteSavedHistoryHandler(
    ICommandBus commandBus,
    IPeerHelper peerHelper,
    IAccessHashHelper accessHashHelper,
    IRandomHelper randomHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestDeleteSavedHistory, MyTelegram.Schema.Messages.IAffectedHistory>,
        Messages.IDeleteSavedHistoryHandler
{
    protected override async Task<IAffectedHistory> HandleCoreAsync(IRequestInput input,
        RequestDeleteSavedHistory obj)
    {
        await accessHashHelper.CheckAccessHashAsync(input, obj.Peer);
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);

        var command = new ClearHistoryCommand(
            DialogId.Create(input.UserId, peer.PeerType, peer.PeerId),
            input.ToRequestInfo(),
            false,
            string.Empty,
            randomHelper.NextInt64(),
            new List<int>(),
            obj.MaxId);
        await commandBus.PublishAsync(command);

        return null!;
    }
}
