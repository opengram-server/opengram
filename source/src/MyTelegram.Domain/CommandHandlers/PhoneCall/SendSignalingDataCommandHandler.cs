using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Commands.PhoneCall;

namespace MyTelegram.Domain.CommandHandlers.PhoneCall;

public class SendSignalingDataCommandHandler : CommandHandler<PhoneCallAggregate, PhoneCallId, SendSignalingDataCommand>
{
    public override Task ExecuteAsync(PhoneCallAggregate aggregate,
        SendSignalingDataCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.ReceiveSignalingData(
            command.RequestInfo,
            command.Data);

        return Task.CompletedTask;
    }
}
