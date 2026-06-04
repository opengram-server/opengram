using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Commands.PhoneCall;

namespace MyTelegram.Domain.CommandHandlers.PhoneCall;

public class DiscardCallCommandHandler : CommandHandler<PhoneCallAggregate, PhoneCallId, DiscardCallCommand>
{
    public override Task ExecuteAsync(PhoneCallAggregate aggregate,
        DiscardCallCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.DiscardCall(
            command.RequestInfo,
            command.Reason,
            command.Duration,
            command.Date,
            command.IsVideo);

        return Task.CompletedTask;
    }
}
