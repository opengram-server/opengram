namespace MyTelegram.Core;

public record UserLoggedOutEvent(long UserId, long TempAuthKeyId, long PermAuthKeyId);