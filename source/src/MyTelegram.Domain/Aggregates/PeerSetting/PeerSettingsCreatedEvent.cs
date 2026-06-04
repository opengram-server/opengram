namespace MyTelegram.Domain.Aggregates.PeerSetting;

public class PeerSettingsCreatedEvent(PeerSettings peerSettings)
    : AggregateEvent<PeerSettingsAggregate, PeerSettingsId>
{
    public PeerSettings PeerSettings { get; } = peerSettings;
}