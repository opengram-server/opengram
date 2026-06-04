using MyTelegram.Domain.Aggregates.ChatTheme;
using MyTelegram.Domain.Events.ChatTheme;

namespace MyTelegram.ReadModel.ReadModelLocators;

public class ChatThemeReadModelLocator : IReadModelLocator
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case IDomainEvent<ChatThemeAggregate, ChatThemeId, ChatThemeSetEvent> e:
                yield return e.AggregateIdentity.Value;
                break;
            case IDomainEvent<ChatThemeAggregate, ChatThemeId, ChatThemeClearedEvent> e:
                yield return e.AggregateIdentity.Value;
                break;
        }
    }
}
