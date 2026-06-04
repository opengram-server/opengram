namespace MyTelegram.Core;
public record BindUserIdToAuthKeySuccessEvent(long TempAuthKeyId, long PermAuthKeyId, long UserId);