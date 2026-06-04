namespace MyTelegram.Domain.Aggregates.PeerSetting;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<PeerSettingsId>))]
public class PeerSettingsId(string value) : Identity<PeerSettingsId>(value)
{
    public static PeerSettingsId Create(long userId, long targetPeerId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"peersettings-{userId}-{targetPeerId}");
    }
}