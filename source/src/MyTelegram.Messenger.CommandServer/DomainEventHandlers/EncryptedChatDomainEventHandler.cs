using MyTelegram.Domain.Aggregates.EncryptedChat;
using MyTelegram.Domain.Events.EncryptedChat;
using MyTelegram.Messenger.DomainEventHandlers;
using MyTelegram.Schema;

namespace MyTelegram.Messenger.CommandServer.DomainEventHandlers;

public class EncryptedChatDomainEventHandler(
    IObjectMessageSender objectMessageSender,
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    IAckCacheService ackCacheService)
    : DomainEventHandlerBase(objectMessageSender, commandBus, idGenerator, ackCacheService),
        ISubscribeSynchronousTo<EncryptedChatAggregate, EncryptedChatId, EncryptionRequestedEvent>,
        ISubscribeSynchronousTo<EncryptedChatAggregate, EncryptedChatId, EncryptionAcceptedEvent>,
        ISubscribeSynchronousTo<EncryptedChatAggregate, EncryptedChatId, EncryptionDiscardedEvent>,
        ISubscribeSynchronousTo<EncryptedChatAggregate, EncryptedChatId, EncryptedMessageSentEvent>
{
    public async Task HandleAsync(
        IDomainEvent<EncryptedChatAggregate, EncryptedChatId, EncryptionRequestedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;

        // Send encryptedChatRequested to participant (User B)
        var encryptedChatRequested = new TEncryptedChatRequested
        {
            Id = evt.ChatId,
            AccessHash = evt.AccessHash,
            Date = evt.Date,
            AdminId = evt.AdminId,
            ParticipantId = evt.ParticipantId,
            GA = new ReadOnlyMemory<byte>(evt.GA)
        };

        var updateEncryption = new TUpdateEncryption
        {
            Chat = encryptedChatRequested,
            Date = evt.Date
        };

        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate> { updateEncryption },
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = evt.Date,
            Seq = 0
        };

        // Push to participant B on all their devices
        await PushUpdatesToPeerAsync(
            new Peer(PeerType.User, evt.ParticipantId),
            updates,
            updatesType: UpdatesType.Updates
        );
    }

    public async Task HandleAsync(
        IDomainEvent<EncryptedChatAggregate, EncryptedChatId, EncryptionAcceptedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        var date = DateTime.UtcNow.ToTimestamp();

        // Send encryptedChat (fully established) to admin (User A)
        // User A sees g_b in GAOrB field
        var encryptedChat = new TEncryptedChat
        {
            Id = evt.ChatId,
            AccessHash = evt.AccessHash,
            Date = date,
            AdminId = evt.AdminId,
            ParticipantId = evt.ParticipantId,
            GAOrB = new ReadOnlyMemory<byte>(evt.GB),
            KeyFingerprint = evt.KeyFingerprint
        };

        var updateEncryption = new TUpdateEncryption
        {
            Chat = encryptedChat,
            Date = date
        };

        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate> { updateEncryption },
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = date,
            Seq = 0
        };

        // Push to admin A on all their devices
        await PushUpdatesToPeerAsync(
            new Peer(PeerType.User, evt.AdminId),
            updates,
            updatesType: UpdatesType.Updates
        );
    }

    public async Task HandleAsync(
        IDomainEvent<EncryptedChatAggregate, EncryptedChatId, EncryptionDiscardedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        var date = DateTime.UtcNow.ToTimestamp();

        var encryptedChatDiscarded = new TEncryptedChatDiscarded
        {
            Id = evt.ChatId,
            HistoryDeleted = evt.DeleteHistory
        };

        var updateEncryption = new TUpdateEncryption
        {
            Chat = encryptedChatDiscarded,
            Date = date
        };

        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate> { updateEncryption },
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = date,
            Seq = 0
        };

        // Determine the other party
        var senderId = evt.RequestInfo.UserId;
        var otherUserId = senderId == evt.AdminId ? evt.ParticipantId : evt.AdminId;

        // Push to the other participant
        await PushUpdatesToPeerAsync(
            new Peer(PeerType.User, otherUserId),
            updates,
            updatesType: UpdatesType.Updates
        );
    }

    public async Task HandleAsync(
        IDomainEvent<EncryptedChatAggregate, EncryptedChatId, EncryptedMessageSentEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;

        // Determine recipient (the other party)
        var senderId = evt.RequestInfo.UserId;
        var recipientId = senderId == evt.AdminId ? evt.ParticipantId : evt.AdminId;

        // Build the encrypted message TL object for the recipient
        IEncryptedMessage encryptedMsg;
        if (evt.MessageType == SendMessageType.MessageService)
        {
            encryptedMsg = new TEncryptedMessageService
            {
                RandomId = evt.RandomId,
                ChatId = evt.ChatId,
                Date = evt.Date,
                Bytes = new ReadOnlyMemory<byte>(evt.Data)
            };
        }
        else
        {
            encryptedMsg = new TEncryptedMessage
            {
                RandomId = evt.RandomId,
                ChatId = evt.ChatId,
                Date = evt.Date,
                Bytes = new ReadOnlyMemory<byte>(evt.Data),
                File = new TEncryptedFileEmpty()
            };
        }

        // Get qts for the recipient
        var qts = await idGenerator.NextIdAsync(IdType.Qts, recipientId);

        var updateNewEncryptedMessage = new TUpdateNewEncryptedMessage
        {
            Message = encryptedMsg,
            Qts = (int)qts
        };

        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate> { updateNewEncryptedMessage },
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = evt.Date,
            Seq = 0
        };

        // Push to recipient on all devices
        await PushUpdatesToPeerAsync(
            new Peer(PeerType.User, recipientId),
            updates,
            updatesType: UpdatesType.Updates
        );
    }
}
