using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Commands.PhoneCall;

namespace MyTelegram.Domain.CommandHandlers.PhoneCall;

public class RequestCallCommandHandler : CommandHandler<PhoneCallAggregate, PhoneCallId, RequestCallCommand>
{
    public override Task ExecuteAsync(PhoneCallAggregate aggregate,
        RequestCallCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.RequestCall(
            command.RequestInfo,
            command.CallId,
            command.AccessHash,
            command.AdminId,
            command.ParticipantId,
            command.IsVideo,
            command.GAHash,
            command.Protocol,
            command.Date);

        return Task.CompletedTask;
    }
}
