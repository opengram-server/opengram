namespace MyTelegram.Domain.CommandHandlers.Channel;

public class UpdatePaidMessagesPriceCommandHandler : CommandHandler<ChannelAggregate, ChannelId, UpdatePaidMessagesPriceCommand>
{
    public override Task ExecuteAsync(ChannelAggregate aggregate, UpdatePaidMessagesPriceCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdatePaidMessagesPrice(command.RequestInfo, command.SendPaidMessagesStars, command.BroadcastMessagesAllowed, command.LinkedMonoforumId);

        return Task.CompletedTask;
    }
}
