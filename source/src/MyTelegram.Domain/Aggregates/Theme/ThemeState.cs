namespace MyTelegram.Domain.Aggregates.Theme;

public class ThemeState : AggregateState<ThemeAggregate, ThemeId, ThemeState>,
    IApply<Events.Theme.ThemeCreatedEvent>,
    IApply<Events.Theme.ThemeUpdatedEvent>
{
    public long CreatorUserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public long? DocumentId { get; private set; }
    public string? Emoticon { get; private set; }
    public bool IsDefault { get; private set; }
    public bool ForChat { get; private set; }

    public void Apply(Events.Theme.ThemeCreatedEvent aggregateEvent)
    {
        CreatorUserId = aggregateEvent.CreatorUserId;
        Title = aggregateEvent.Title;
        Slug = aggregateEvent.Slug;
        DocumentId = aggregateEvent.DocumentId;
        Emoticon = aggregateEvent.Emoticon;
        IsDefault = aggregateEvent.IsDefault;
        ForChat = aggregateEvent.ForChat;
    }

    public void Apply(Events.Theme.ThemeUpdatedEvent aggregateEvent)
    {
        if (aggregateEvent.Title != null) Title = aggregateEvent.Title;
        if (aggregateEvent.Slug != null) Slug = aggregateEvent.Slug;
        if (aggregateEvent.DocumentId.HasValue) DocumentId = aggregateEvent.DocumentId;
        if (aggregateEvent.Emoticon != null) Emoticon = aggregateEvent.Emoticon;
    }
}
