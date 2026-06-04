namespace MyTelegram.ReadModel.ReadModelLocators;

public class ReadingHistoryReadModelLocator : IReadingHistoryReadModelLocator, ITransientDependency
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        var aggregateEvent = domainEvent.GetAggregateEvent();
        switch (aggregateEvent)
        {
            case ReadInboxMessage2Event readInboxMessage2Event:
                yield return ReadingHistoryId.Create(readInboxMessage2Event.ReaderUserId,
                    readInboxMessage2Event.ToPeer.PeerId, readInboxMessage2Event.MaxMessageId).Value;
                break;
        }

        yield return domainEvent.GetIdentity().Value;
    }
}