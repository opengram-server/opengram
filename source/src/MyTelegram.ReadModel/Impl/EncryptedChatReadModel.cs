using MyTelegram.Domain.Aggregates.EncryptedChat;
using MyTelegram.Domain.Events.EncryptedChat;
using MongoDB.Bson.Serialization.Attributes;

namespace MyTelegram.ReadModel.Impl;

public class EncryptedChatReadModel : IEncryptedChatReadModel,
    IAmReadModelFor<EncryptedChatAggregate, EncryptedChatId, EncryptionRequestedEvent>,
    IAmReadModelFor<EncryptedChatAggregate, EncryptedChatId, EncryptionAcceptedEvent>,
    IAmReadModelFor<EncryptedChatAggregate, EncryptedChatId, EncryptionDiscardedEvent>
{
    [BsonId]
    public virtual string Id { get; set; } = null!;
    public virtual long ChatId { get; private set; }
    public virtual long AccessHash { get; private set; }
    public virtual long AdminId { get; private set; }
    public virtual long ParticipantId { get; private set; }
    public virtual long AdminPermAuthKeyId { get; private set; }
    public virtual long ParticipantPermAuthKeyId { get; private set; }
    public virtual byte[]? Ga { get; private set; }
    public virtual byte[]? Gb { get; private set; }
    public virtual long KeyFingerprint { get; private set; }
    public virtual long RandomId { get; private set; }
    public virtual long? Version { get; set; }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<EncryptedChatAggregate, EncryptedChatId, EncryptionRequestedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        Id = domainEvent.AggregateIdentity.Value;
        ChatId = evt.ChatId;
        AccessHash = evt.AccessHash;
        AdminId = evt.AdminId;
        ParticipantId = evt.ParticipantId;
        AdminPermAuthKeyId = evt.AdminPermAuthKeyId;
        Ga = evt.GA;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<EncryptedChatAggregate, EncryptedChatId, EncryptionAcceptedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        Gb = evt.GB;
        KeyFingerprint = evt.KeyFingerprint;
        ParticipantPermAuthKeyId = evt.ParticipantPermAuthKeyId;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<EncryptedChatAggregate, EncryptedChatId, EncryptionDiscardedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
