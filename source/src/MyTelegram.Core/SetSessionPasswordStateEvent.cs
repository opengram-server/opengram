namespace MyTelegram.Core;

public record SetSessionPasswordStateEvent(long UserId, PasswordState PasswordState);