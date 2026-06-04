namespace MyTelegram.Core;

public record UserIsOnlineEvent(long UserId, long TempAuthKeyId, long PermAuthKeyId);