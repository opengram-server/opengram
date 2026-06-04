using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Commands.PhoneCall;

namespace MyTelegram.Domain.CommandHandlers.PhoneCall;

public class ConfirmCallCommandHandler : CommandHandler<PhoneCallAggregate, PhoneCallId, ConfirmCallCommand>
{
    public override Task ExecuteAsync(PhoneCallAggregate aggregate,
        ConfirmCallCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.ConfirmCall(
            command.RequestInfo,
            command.GA,
            command.KeyFingerprint,
            command.Protocol,
            command.Connections,
            command.StartDate);

        return Task.CompletedTask;
    }
}
