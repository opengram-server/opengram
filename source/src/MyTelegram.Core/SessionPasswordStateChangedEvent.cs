namespace MyTelegram.Core;

public record SessionPasswordStateChangedEvent(long AuthKeyId, PasswordState PasswordState);