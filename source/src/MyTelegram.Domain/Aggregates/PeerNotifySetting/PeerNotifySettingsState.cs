namespace MyTelegram.Domain.Aggregates.PeerNotifySetting;

public class PeerNotifySettingsState :
    AggregateState<PeerNotifySettingsAggregate, PeerNotifySettingsId, PeerNotifySettingsState>,
    IApply<PeerNotifySettingsUpdatedEvent>
{
    public MyTelegram.PeerNotifySettings PeerNotifySettings { get; private set; } = default!;

    public void Apply(PeerNotifySettingsUpdatedEvent aggregateEvent)
    {
        PeerNotifySettings = aggregateEvent.PeerNotifySettings;
    }

    public void LoadSnapshot(PeerNotifySettingsSnapshot snapshot)
    {
        PeerNotifySettings = snapshot.PeerNotifySettings;
    }
}
