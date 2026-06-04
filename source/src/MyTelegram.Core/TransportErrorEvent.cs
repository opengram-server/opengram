namespace MyTelegram.Core;

public record TransportErrorEvent(
    long AuthKeyId,
    string ConnectionId,
    int TransportErrorCode);