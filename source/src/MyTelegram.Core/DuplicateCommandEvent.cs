namespace MyTelegram.Core;
public record DuplicateCommandEvent(long PermAuthKeyId, long UserId, long ReqMsgId);
