using MyTelegram.Domain.Aggregates.Theme;

namespace MyTelegram.Domain.Events.Theme;

public class ThemeCreatedEvent : AggregateEvent<ThemeAggregate, ThemeId>
{
    public RequestInfo RequestInfo { get; }
    public long CreatorUserId { get; }
    public string Title { get; }
    public string Slug { get; }
    public long? DocumentId { get; }
    public string? Emoticon { get; }
    public bool IsDefault { get; }
    public bool ForChat { get; }

    public ThemeCreatedEvent(RequestInfo requestInfo,
        long creatorUserId,
        string title,
        string slug,
        long? documentId,
        string? emoticon,
        bool isDefault,
        bool forChat)
    {
        RequestInfo = requestInfo;
        CreatorUserId = creatorUserId;
        Title = title;
        Slug = slug;
        DocumentId = documentId;
        Emoticon = emoticon;
        IsDefault = isDefault;
        ForChat = forChat;
    }
}
