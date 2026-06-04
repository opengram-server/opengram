using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Commands.PhoneCall;

namespace MyTelegram.Domain.CommandHandlers.PhoneCall;

public class AcceptCallCommandHandler : CommandHandler<PhoneCallAggregate, PhoneCallId, AcceptCallCommand>
{
    public override Task ExecuteAsync(PhoneCallAggregate aggregate,
        AcceptCallCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.AcceptCall(
            command.RequestInfo,
            command.GB,
            command.Protocol);

        return Task.CompletedTask;
    }
}
