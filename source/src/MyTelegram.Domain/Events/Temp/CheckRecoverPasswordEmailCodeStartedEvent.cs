namespace MyTelegram.Domain.Events.Temp;

public class CheckRecoverPasswordEmailCodeStartedEvent(RequestInfo requestInfo,
    SrpData? srpData,
    byte[]? newPasswordHash,
    string code,
    string? hint,
    string? email
    ) : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public string Code { get; } = code;
    public string? Email { get; } = email;
    public string? Hint { get; } = hint;
    public byte[]? NewPasswordHash { get; } = newPasswordHash;
    public SrpData? SrpData { get; } = srpData;
}