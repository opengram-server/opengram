namespace MyTelegram.Core;

public record AuthKeyUnRegisteredIntegrationEvent(
    long PermAuthKeyId,
    long TempAuthKeyId);