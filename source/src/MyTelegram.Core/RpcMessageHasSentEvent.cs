namespace MyTelegram.Core;

public record RpcMessageHasSentEvent(long ReqMsgId, long UserId, long MsgId, long GlobalSeqNo);