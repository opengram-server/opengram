namespace MyTelegram.ReadModel.ReadModelLocators;

public class InstalledStickerSetReadModelLocator : IInstalledStickerSetReadModelLocator, ITransientDependency
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        var aggregateEvent = domainEvent.GetAggregateEvent();
        if (aggregateEvent is StickerSetInstalledEvent installedEvent)
        {
            // Create unique ID matching event handler format: {userId}_{stickerSetId}
            yield return $"{installedEvent.UserId}_{installedEvent.StickerSetId}";
        }
    }
}
