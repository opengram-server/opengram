using MyTelegram.Domain.Aggregates.ChatTheme;
using MyTelegram.Domain.Events.ChatTheme;

namespace MyTelegram.ReadModel.Impl;

public class ChatThemeReadModel : IChatThemeReadModel,
    IAmReadModelFor<ChatThemeAggregate, ChatThemeId, ChatThemeSetEvent>,
    IAmReadModelFor<ChatThemeAggregate, ChatThemeId, ChatThemeClearedEvent>
{
    public string Id { get; private set; } = null!;
    public long UserId { get; private set; }
    public long ChatId { get; private set; }
    public long ThemeId { get; private set; }
    public string Emoticon { get; private set; } = string.Empty;
    public int? MessageId { get; private set; }
    public int SetDate { get; private set; }
    public long? Version { get; set; }

    public Task ApplyAsync(IReadModelContext context, 
        IDomainEvent<ChatThemeAggregate, ChatThemeId, ChatThemeSetEvent> domainEvent, 
        CancellationToken cancellationToken)
    {
        Id = domainEvent.AggregateIdentity.Value;
        UserId = domainEvent.AggregateEvent.OwnerPeerId;
        ChatId = domainEvent.AggregateEvent.PeerId;
        ThemeId = 0; // Will be set based on emoticon lookup
        Emoticon = domainEvent.AggregateEvent.Emoticon;
        MessageId = domainEvent.AggregateEvent.MessageId;
        SetDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, 
        IDomainEvent<ChatThemeAggregate, ChatThemeId, ChatThemeClearedEvent> domainEvent, 
        CancellationToken cancellationToken)
    {
        Emoticon = string.Empty;
        MessageId = null;
        ThemeId = 0;

        return Task.CompletedTask;
    }
}
