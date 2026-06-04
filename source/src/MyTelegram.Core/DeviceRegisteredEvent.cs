namespace MyTelegram.Core;

public record DeviceRegisteredEvent(long AuthKeyId, long PermAuthKeyId, long SessionId);