namespace MyTelegram.Core;

public record UserSignUpSuccessIntegrationEvent(
    long TempAuthKeyId,
    long PermAuthKeyId,
    long UserId);