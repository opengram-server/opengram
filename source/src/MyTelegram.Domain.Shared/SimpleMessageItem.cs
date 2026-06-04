namespace MyTelegram;

public record SimpleMessageItem(long OwnerPeerId, int MessageId, PeerType ToPeerType, long ToPeerId);