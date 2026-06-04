namespace MyTelegram.Domain.Commands.Channel;

public class ToggleSignatureCommand(ChannelId aggregateId, RequestInfo requestInfo, bool signatureEnabled, bool profilesEnabled)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public bool SignatureEnabled { get; } = signatureEnabled;
    public bool ProfilesEnabled { get; } = profilesEnabled;
}