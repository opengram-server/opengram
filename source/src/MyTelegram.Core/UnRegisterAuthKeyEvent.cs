namespace MyTelegram.Core;

public record UnRegisterAuthKeyEvent(long PermAuthKeyId, long UserId);

public record SessionRevokedEvent(long PermAuthKeyId, long UserId, List<long> RevokedPermAuthKeyIdList);