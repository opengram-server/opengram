using MyTelegram.Domain.Aggregates.Theme;

namespace MyTelegram.Domain.Events.Theme;

public class ThemeUpdatedEvent : AggregateEvent<ThemeAggregate, ThemeId>
{
    public RequestInfo RequestInfo { get; }
    public string? Title { get; }
    public string? Slug { get; }
    public long? DocumentId { get; }
    public string? Emoticon { get; }

    public ThemeUpdatedEvent(RequestInfo requestInfo,
        string? title,
        string? slug,
        long? documentId,
        string? emoticon)
    {
        RequestInfo = requestInfo;
        Title = title;
        Slug = slug;
        DocumentId = documentId;
        Emoticon = emoticon;
    }
}
