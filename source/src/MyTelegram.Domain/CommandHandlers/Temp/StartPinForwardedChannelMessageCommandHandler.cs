namespace MyTelegram.Domain.CommandHandlers.Temp;

public class
    StartPinForwardedChannelMessageCommandHandler : CommandHandler<TempAggregate, TempId,
    StartPinForwardedChannelMessageCommand>
{
    public override Task ExecuteAsync(TempAggregate aggregate, StartPinForwardedChannelMessageCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.StartPinForwardedChannelMessage(command.RequestInfo, command.ChannelId, command.MessageId);

        return Task.CompletedTask;
    }
}