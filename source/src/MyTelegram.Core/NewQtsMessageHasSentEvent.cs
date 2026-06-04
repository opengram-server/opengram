namespace MyTelegram.Core;

public record NewQtsMessageHasSentEvent(
    long UserId,
    Peer ToPeer,
    long MsgId,
    long TempAuthKeyId,
    long PermAuthKeyId,
    int Qts,
    long GlobalSeqNo,
    long ReqMsgId);