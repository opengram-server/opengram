// ReSharper disable All

using MyTelegram.Queries.ScheduledMessage;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Get scheduled messages
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHAT_ADMIN_REQUIRED You must be an admin in this chat to do this.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// See <a href="https://corefork.telegram.org/method/messages.getScheduledMessages" />
///</summary>
internal sealed class GetScheduledMessagesHandler(
    IQueryProcessor queryProcessor,
    IPeerHelper peerHelper,
    IUserConverterService userConverterService) 
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetScheduledMessages, MyTelegram.Schema.Messages.IMessages>,
    Messages.IGetScheduledMessagesHandler
{
    protected override async Task<MyTelegram.Schema.Messages.IMessages> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetScheduledMessages obj)
    {
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);
        var ownerPeerId = peer.PeerType == PeerType.Channel ? peer.PeerId : input.UserId;
        
        // Get scheduled messages from database
        var scheduledMessages = await queryProcessor.ProcessAsync(
            new GetScheduledMessagesQuery(ownerPeerId, obj.Id.ToList()),
            CancellationToken.None);
        
        if (scheduledMessages.Count == 0)
        {
            return new TMessages
            {
                Messages = new TVector<IMessage>(),
                Chats = new TVector<IChat>(),
                Users = new TVector<IUser>()
            };
        }
        
        // Convert scheduled messages to Message objects
        var messages = new List<IMessage>();
        var userIds = new HashSet<long> { input.UserId };
        var channelIds = new HashSet<long>();
        
        foreach (var scheduled in scheduledMessages)
        {
            var message = ConvertScheduledToMessage(scheduled);
            messages.Add(message);
            
            // Collect user and channel IDs for fetching
            userIds.Add(scheduled.SenderUserId);
            if (scheduled.ToPeer.PeerType == PeerType.Channel)
            {
                channelIds.Add(scheduled.ToPeer.PeerId);
            }
            if (scheduled.SendAs?.PeerType == PeerType.Channel)
            {
                channelIds.Add(scheduled.SendAs.PeerId);
            }
        }
        
        // Fetch users
        var users = await userConverterService.GetUserListAsync(
            input.ToRequestInfo(),
            userIds.ToList(),
            layer: input.Layer);
        
        return new TMessages
        {
            Messages = new TVector<IMessage>(messages),
            Chats = new TVector<IChat>(),
            Users = new TVector<IUser>(users)
        };
    }
    
    private IMessage ConvertScheduledToMessage(IScheduledMessageReadModel scheduled)
    {
        return new TMessage
        {
            Out = true,
            Id = scheduled.ScheduleMessageId,
            FromId = new TPeerUser { UserId = scheduled.SenderUserId },
            PeerId = scheduled.ToPeer.ToPeer(),
            Message = scheduled.Message,
            Date = scheduled.ScheduleDate,
            Entities = scheduled.Entities,
            Media = scheduled.Media,
            ReplyTo = scheduled.ReplyTo?.ToMessageReplyHeader(),
            ReplyMarkup = scheduled.ReplyMarkup,
            FromScheduled = true,
            Silent = scheduled.Silent,
            InvertMedia = scheduled.InvertMedia,
            Effect = scheduled.Effect
        };
    }
}
