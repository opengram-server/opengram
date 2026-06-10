using MyTelegram.Domain.Aggregates.Theme;

namespace MyTelegram.Domain.Commands.Theme;

public class CreateThemeCommand : Command<ThemeAggregate, ThemeId, IExecutionResult>
{
    public RequestInfo RequestInfo { get; }
    public long CreatorUserId { get; }
    public string Title { get; }
    public string Slug { get; }
    public long? DocumentId { get; }
    public string? Emoticon { get; }
    public bool IsDefault { get; }
    public bool ForChat { get; }

    public CreateThemeCommand(ThemeId aggregateId,
        RequestInfo requestInfo,
        long creatorUserId,
        string title,
        string slug,
        long? documentId,
        string? emoticon,
        bool isDefault,
        bool forChat) : base(aggregateId)
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

public class UpdateThemeCommand : Command<ThemeAggregate, ThemeId, IExecutionResult>
{
    public RequestInfo RequestInfo { get; }
    public string? Title { get; }
    public string? Slug { get; }
    public long? DocumentId { get; }
    public string? Emoticon { get; }

    public UpdateThemeCommand(ThemeId aggregateId,
        RequestInfo requestInfo,
        string? title,
        string? slug,
        long? documentId,
        string? emoticon) : base(aggregateId)
    {
        RequestInfo = requestInfo;
        Title = title;
        Slug = slug;
        DocumentId = documentId;
        Emoticon = emoticon;
    }
}
