using MyTelegram.Domain.Extensions;

namespace MyTelegram.ReadModel.Impl;

public class ScheduledMessageReadModel : IScheduledMessageReadModel,
    IAmReadModelFor<MessageAggregate, MessageId, OutboxMessageCreatedEvent>,
    IAmReadModelFor<MessageAggregate, MessageId, ScheduledMessageConvertedToRegularEvent>
{
    public virtual string Id { get; set; } = default!;
    public virtual long? Version { get; set; }
    
    public long OwnerPeerId { get; private set; }
    public int ScheduleMessageId { get; private set; }
    public Peer ToPeer { get; private set; } = default!;
    public long SenderUserId { get; private set; }
    public Peer SenderPeer { get; private set; } = default!;
    public string Message { get; private set; } = string.Empty;
    public int Date { get; private set; }
    public int ScheduleDate { get; private set; }
    public long RandomId { get; private set; }
    public TVector<IMessageEntity>? Entities { get; private set; }
    public IMessageMedia? Media { get; private set; }
    public IInputReplyTo? ReplyTo { get; private set; }
    public IReplyMarkup? ReplyMarkup { get; private set; }
    public Peer? SendAs { get; private set; }
    public bool Silent { get; private set; }
    public long? Effect { get; private set; }
    public bool InvertMedia { get; private set; }
    public long? GroupedId { get; private set; }
    public MessageType MessageType { get; private set; }
    public SendMessageType SendMessageType { get; private set; }
    public bool IsSent { get; private set; }
    public int? ActualMessageId { get; private set; }
    public int? SentDate { get; private set; }

    public Task ApplyAsync(IReadModelContext context, 
        IDomainEvent<MessageAggregate, MessageId, OutboxMessageCreatedEvent> domainEvent, 
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        var messageItem = evt.OutboxMessageItem;
        
        // Only create scheduled message read model if message has schedule date
        if (!messageItem.ScheduleDate.HasValue || !messageItem.ScheduleMessageId.HasValue)
        {
            return Task.CompletedTask;
        }

        Id = ScheduledMessageId.Create(messageItem.OwnerPeer.PeerId, messageItem.ScheduleMessageId.Value);
        OwnerPeerId = messageItem.OwnerPeer.PeerId;
        ScheduleMessageId = messageItem.ScheduleMessageId.Value;
        ToPeer = messageItem.ToPeer;
        SenderUserId = messageItem.SenderUserId;
        SenderPeer = messageItem.SenderPeer;
        Message = messageItem.Message;
        Date = messageItem.Date;
        ScheduleDate = messageItem.ScheduleDate.Value;
        
        Console.WriteLine($"*** SCHEDULED MESSAGE CREATED: ScheduleDate={ScheduleDate}, CurrentTime={DateTime.UtcNow.ToTimestamp()} ***");
        RandomId = messageItem.RandomId;
        Entities = messageItem.Entities;
        Media = messageItem.Media;
        ReplyTo = messageItem.InputReplyTo;
        ReplyMarkup = messageItem.ReplyMarkup;
        SendAs = messageItem.SendAs;
        Silent = messageItem.Silent;
        Effect = messageItem.Effect;
        InvertMedia = messageItem.InvertMedia;
        GroupedId = messageItem.GroupId;
        MessageType = messageItem.MessageType;
        SendMessageType = messageItem.SendMessageType;
        IsSent = false;
        ActualMessageId = null;
        SentDate = null;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<MessageAggregate, MessageId, ScheduledMessageConvertedToRegularEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        
        // Mark scheduled message as sent
        IsSent = true;
        ActualMessageId = evt.MessageId;
        SentDate = (int)(DateTime.UtcNow.ToTimestamp() / 1000);

        return Task.CompletedTask;
    }
}
