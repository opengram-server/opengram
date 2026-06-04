namespace MyTelegram.Core;

public record PingTimeoutEvent(string ConnectionId,long AuthKeyId);