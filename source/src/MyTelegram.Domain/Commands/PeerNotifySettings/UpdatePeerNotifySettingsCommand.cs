namespace MyTelegram.Domain.Commands.PeerNotifySettings;

public class
    UpdatePeerNotifySettingsCommand(
        PeerNotifySettingsId aggregateId,
        RequestInfo requestInfo,
        long ownerPeerId,
        PeerType peerType,
        long peerId,
        bool? showPreviews,
        bool? silent,
        int? muteUntil,
        string? sound)
    : RequestCommand2<PeerNotifySettingsAggregate, PeerNotifySettingsId,
        IExecutionResult>(aggregateId, requestInfo)
{
    public int? MuteUntil { get; } = muteUntil; // = int.MaxValue;
    public long OwnerPeerId { get; } = ownerPeerId;
    public long PeerId { get; } = peerId;
    public PeerType PeerType { get; } = peerType;
    public bool? ShowPreviews { get; } = showPreviews; // = true;
    public bool? Silent { get; } = silent;
    public string? Sound { get; } = sound; // = "default";
}
