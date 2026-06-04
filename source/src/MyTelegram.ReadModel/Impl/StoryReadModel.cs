using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.Aggregates.Story.Events;

namespace MyTelegram.ReadModel.Impl;

public class StoryReadModel : IStoryReadModel,
    IAmReadModelFor<StoryAggregate, StoryId, StoryCreatedEvent>,
    IAmReadModelFor<StoryAggregate, StoryId, StoryViewIncrementedEvent>,
    IAmReadModelFor<StoryAggregate, StoryId, StoryEditedEvent>,
    IAmReadModelFor<StoryAggregate, StoryId, StoryDeletedEvent>,
    IAmReadModelFor<StoryAggregate, StoryId, StoryPinnedEvent>
{
    public string Id { get; set; } = null!;
    public long OwnerPeerId { get; private set; }
    public int StoryId { get; private set; }
    public IMessageMedia Media { get; private set; } = new TMessageMediaEmpty();
    public long RandomId { get; private set; }
    public List<PrivacyValueData> PrivacyRules { get; private set; } = new();
    public int Date { get; private set; }
    public int ExpireDate { get; private set; }
    public Peer? FromPeer { get; private set; }
    public string? Caption { get; private set; }
    public List<IMediaArea>? MediaAreas { get; private set; }
    public bool Pinned { get; private set; }
    public bool NoForwards { get; private set; }
    public List<IMessageEntity>? Entities { get; private set; }
    public int? Period { get; private set; }
    public Peer? FwdFromId { get; private set; }
    public int? FwdFromStory { get; private set; }
    public bool Archived { get; private set; }
    
    // Additional fields for internal use
    public int ViewsCount { get; private set; }
    public bool IsDeleted { get; private set; }
    public long? Version { get; set; }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<StoryAggregate, StoryId, StoryCreatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        Id = domainEvent.AggregateIdentity.Value;
        OwnerPeerId = domainEvent.AggregateEvent.PeerId;
        StoryId = domainEvent.AggregateEvent.StoryId;
        // Media = new TMessageMediaPhoto(); // TODO: Convert byte[] to IMessageMedia
        Caption = domainEvent.AggregateEvent.Caption;
        // PrivacyRules = Convert privacy rules
        Date = domainEvent.AggregateEvent.Date;
        ExpireDate = domainEvent.AggregateEvent.ExpireDate;
        Pinned = domainEvent.AggregateEvent.Pinned;
        NoForwards = domainEvent.AggregateEvent.NoForwards;
        ViewsCount = 0;
        IsDeleted = false;
        Archived = false;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<StoryAggregate, StoryId, StoryViewIncrementedEvent> domainEvent, CancellationToken cancellationToken)
    {
        ViewsCount = domainEvent.AggregateEvent.ViewsCount;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<StoryAggregate, StoryId, StoryEditedEvent> domainEvent, CancellationToken cancellationToken)
    {
        // TODO: Media and PrivacyRules are stored as byte[]/List<long> in event, need conversion
        // For now, only update caption
        if (domainEvent.AggregateEvent.Caption != null)
            Caption = domainEvent.AggregateEvent.Caption;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<StoryAggregate, StoryId, StoryDeletedEvent> domainEvent, CancellationToken cancellationToken)
    {
        IsDeleted = true;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<StoryAggregate, StoryId, StoryPinnedEvent> domainEvent, CancellationToken cancellationToken)
    {
        Pinned = domainEvent.AggregateEvent.Pinned;
        return Task.CompletedTask;
    }
}
