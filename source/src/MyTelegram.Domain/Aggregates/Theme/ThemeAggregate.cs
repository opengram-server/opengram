using MyTelegram.Domain.Events.Theme;

namespace MyTelegram.Domain.Aggregates.Theme;

public class ThemeAggregate : AggregateRoot<ThemeAggregate, ThemeId>
{
    private readonly ThemeState _state = new();

    public ThemeAggregate(ThemeId id) : base(id)
    {
        Register(_state);
    }

    public void Create(RequestInfo requestInfo,
        long creatorUserId,
        string title,
        string slug,
        long? documentId,
        string? emoticon,
        bool isDefault,
        bool forChat)
    {
        Specs.AggregateIsNew.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new ThemeCreatedEvent(requestInfo, creatorUserId, title, slug, documentId, emoticon, isDefault, forChat));
    }

    public void Update(RequestInfo requestInfo,
        string? title,
        string? slug,
        long? documentId,
        string? emoticon)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new ThemeUpdatedEvent(requestInfo, title, slug, documentId, emoticon));
    }
}
