namespace MyTelegram.Domain.Aggregates.Channel;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<ChannelId>))]
public class ChannelId(string value) : Identity<ChannelId>(value)
{
    public static ChannelId Create(long channelId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"channel_{channelId}");
    }
}
