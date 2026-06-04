namespace MyTelegram.Core;

public record CreateEncryptedPushMessageEvent(
    long InboxOwnerPeerId,
    byte[] Data,
    int Qts,
    long InboxOwnerPermAuthKeyId);