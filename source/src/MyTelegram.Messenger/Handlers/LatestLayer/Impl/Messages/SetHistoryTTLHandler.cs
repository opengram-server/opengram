// ReSharper disable All

using EventFlow.Exceptions;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Set maximum Time-To-Live of all messages in the specified chat
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHAT_NOT_MODIFIED No changes were made to chat information because the new information you passed is identical to the current information.
/// 400 TTL_PERIOD_INVALID The specified TTL period is invalid.
/// See <a href="https://corefork.telegram.org/method/messages.setHistoryTTL" />
///</summary>
internal sealed class SetHistoryTTLHandler(
    ICommandBus commandBus,
    IPeerHelper peerHelper) 
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSetHistoryTTL, MyTelegram.Schema.IUpdates>,
    Messages.ISetHistoryTTLHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestSetHistoryTTL obj)
    {
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);
        var ttlPeriod = obj.Period == 0 ? null : (int?)obj.Period;
        
        // Validate TTL period (max 1 year = 31536000 seconds)
        if (obj.Period < 0 || obj.Period > 31536000)
        {
            RpcErrors.RpcErrors400.TtlPeriodInvalid.ThrowRpcError();
        }
        
        // Set TTL based on peer type
        if (peer.PeerType == PeerType.Channel)
        {
            var command = new SetChannelHistoryTTLCommand(
                ChannelId.Create(peer.PeerId),
                input.ToRequestInfo(),
                ttlPeriod);
            await commandBus.PublishAsync(command, CancellationToken.None);
        }
        else
        {
            // Create dialog if it doesn't exist, then set TTL
            var dialogId = DialogId.Create(input.UserId, peer);
            
            // Create dialog first (will be ignored if already exists)
            var createCommand = new CreateDialogCommand(
                dialogId,
                input.ToRequestInfo(),
                input.UserId,
                peer,
                0, // channelHistoryMinId
                0, // topMessageId
                ttlPeriod);
            
            try
            {
                await commandBus.PublishAsync(createCommand, CancellationToken.None);
            }
            catch (DomainError)
            {
                // Dialog already exists, just set TTL
                var command = new SetDialogHistoryTTLCommand(
                    dialogId,
                    input.ToRequestInfo(),
                    peer,
                    ttlPeriod);
                await commandBus.PublishAsync(command, CancellationToken.None);
            }
            
            // If peer is a user (not chat), also create/set TTL for the other user's dialog
            if (peer.PeerType == PeerType.User)
            {
                var otherUserDialogId = DialogId.Create(peer.PeerId, new Peer(PeerType.User, input.UserId));
                var otherUserPeer = new Peer(PeerType.User, input.UserId);
                
                var otherCreateCommand = new CreateDialogCommand(
                    otherUserDialogId,
                    input.ToRequestInfo(),
                    peer.PeerId,
                    otherUserPeer,
                    0,
                    0,
                    ttlPeriod);
                
                try
                {
                    await commandBus.PublishAsync(otherCreateCommand, CancellationToken.None);
                }
                catch (DomainError)
                {
                    // Dialog already exists, just set TTL
                    var otherUserCommand = new SetDialogHistoryTTLCommand(
                        otherUserDialogId,
                        input.ToRequestInfo(),
                        otherUserPeer,
                        ttlPeriod);
                    await commandBus.PublishAsync(otherUserCommand, CancellationToken.None);
                }
            }
        }
        
        // Return update to notify client
        var update = new TUpdatePeerHistoryTTL
        {
            Peer = peer.ToPeer(),
            TtlPeriod = ttlPeriod
        };
        
        return new TUpdates
        {
            Updates = new TVector<IUpdate>(update),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = CurrentDate
        };
    }
}
