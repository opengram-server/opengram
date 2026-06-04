namespace MyTelegram.ReadModel.ReadModelLocators;

public class ChatReadModelLocator : IReadModelLocator, ITransientDependency
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        return new[] { domainEvent.GetIdentity().Value };
    }
}
