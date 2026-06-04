namespace MyTelegram.Domain.Commands.PushDevice;

public class UnRegisterDeviceCommand(
    PushDeviceId aggregateId,
    RequestInfo requestInfo,
    int tokenType,
    string token,
    IReadOnlyList<long> otherUids)
    : RequestCommand2<PushDeviceAggregate, PushDeviceId, IExecutionResult>(aggregateId, requestInfo)
{
    public IReadOnlyList<long> OtherUids { get; } = otherUids;
    public string Token { get; } = token;
    public int TokenType { get; } = tokenType;
}
