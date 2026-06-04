namespace MyTelegram.Core;

public record ClientDisconnectedEvent(
    string ConnectionId,
    long AuthKeyId,
    long SessionId);