namespace MyTelegram.Core;

public record PhoneCallDataReceivedEvent(
    uint ObjectId,
    long UserId,
    long ReqMsgId,
    int SeqNumber,
    long AuthKeyId,
    long PermAuthKeyId,
    byte[] Data,
    int Layer);