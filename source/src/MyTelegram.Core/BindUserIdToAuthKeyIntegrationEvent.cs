namespace MyTelegram.Core;

public record BindUserIdToAuthKeyIntegrationEvent(
    long AuthKeyId,
    long PermAuthKeyId,
    long UserId);