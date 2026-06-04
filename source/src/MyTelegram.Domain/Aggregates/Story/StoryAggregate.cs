using MyTelegram.Domain.Aggregates.Story.Events;

namespace MyTelegram.Domain.Aggregates.Story;

public class StoryAggregate : AggregateRoot<StoryAggregate, StoryId>,
    IApply<StoryCreatedEvent>,
    IApply<StoryViewIncrementedEvent>,
    IApply<StoryReactionAddedEvent>,
    IApply<StoryEditedEvent>,
    IApply<StoryDeletedEvent>,
    IApply<StoryPinnedEvent>
{
    private readonly StoryState _state = new();

    public StoryAggregate(StoryId id) : base(id)
    {
        Register(_state);
    }

    public void CreateStory(
        long peerId,
        int storyId,
        byte[] media,
        string? caption,
        List<long>? privacyRules,
        int date,
        int expireDate,
        bool pinned,
        bool noForwards,
        bool isPublic)
    {
        Emit(new StoryCreatedEvent(
            peerId,
            storyId,
            media,
            caption,
            privacyRules,
            date,
            expireDate,
            pinned,
            noForwards,
            isPublic));
    }

    public void IncrementViews(int viewsCount)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new StoryViewIncrementedEvent(_state.PeerId, _state.StoryId, viewsCount));
    }

    public void AddReaction(long userId, string reaction)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new StoryReactionAddedEvent(_state.PeerId, _state.StoryId, userId, reaction));
    }

    public void EditStory(byte[]? media, string? caption, List<long>? privacyRules)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new StoryEditedEvent(_state.PeerId, _state.StoryId, media, caption, privacyRules));
    }

    public void DeleteStory()
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new StoryDeletedEvent(_state.PeerId, _state.StoryId));
    }

    public void TogglePinned(bool pinned)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new StoryPinnedEvent(_state.PeerId, _state.StoryId, pinned));
    }

    public void Apply(StoryCreatedEvent aggregateEvent)
    {
        _state.PeerId = aggregateEvent.PeerId;
        _state.StoryId = aggregateEvent.StoryId;
        _state.Media = aggregateEvent.Media;
        _state.Caption = aggregateEvent.Caption;
        _state.PrivacyRules = aggregateEvent.PrivacyRules;
        _state.Date = aggregateEvent.Date;
        _state.ExpireDate = aggregateEvent.ExpireDate;
        _state.Pinned = aggregateEvent.Pinned;
        _state.NoForwards = aggregateEvent.NoForwards;
        _state.IsPublic = aggregateEvent.IsPublic;
        _state.ViewsCount = 0;
        _state.IsDeleted = false;
    }

    public void Apply(StoryViewIncrementedEvent aggregateEvent)
    {
        _state.ViewsCount = aggregateEvent.ViewsCount;
    }

    public void Apply(StoryReactionAddedEvent aggregateEvent)
    {
        // Reactions stored in separate collection
    }

    public void Apply(StoryEditedEvent aggregateEvent)
    {
        if (aggregateEvent.Media != null)
            _state.Media = aggregateEvent.Media;
        if (aggregateEvent.Caption != null)
            _state.Caption = aggregateEvent.Caption;
        if (aggregateEvent.PrivacyRules != null)
            _state.PrivacyRules = aggregateEvent.PrivacyRules;
    }

    public void Apply(StoryDeletedEvent aggregateEvent)
    {
        _state.IsDeleted = true;
    }

    public void Apply(StoryPinnedEvent aggregateEvent)
    {
        _state.Pinned = aggregateEvent.Pinned;
    }
}
