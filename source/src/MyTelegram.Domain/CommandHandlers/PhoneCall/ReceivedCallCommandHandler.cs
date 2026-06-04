using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Commands.PhoneCall;

namespace MyTelegram.Domain.CommandHandlers.PhoneCall;

public class ReceivedCallCommandHandler : CommandHandler<PhoneCallAggregate, PhoneCallId, ReceivedCallCommand>
{
    public override Task ExecuteAsync(PhoneCallAggregate aggregate,
        ReceivedCallCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.MarkAsReceived(
            command.RequestInfo,
            command.ReceiveDate);

        return Task.CompletedTask;
    }
}
