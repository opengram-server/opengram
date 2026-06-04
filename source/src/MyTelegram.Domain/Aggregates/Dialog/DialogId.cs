namespace MyTelegram.Domain.Aggregates.Dialog;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<DialogId>))]
public class DialogId(string value) : Identity<DialogId>(value)
{
    public static DialogId Create(long ownerId,
        PeerType toPeerType,
        long toPeerId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands,
            $"dialog_{ownerId}_{toPeerType}_{toPeerId}");
    }

    public static DialogId Create(long ownerId,
        Peer toPeer)
    {
        return Create(ownerId, toPeer.PeerType, toPeer.PeerId);
    }
}
