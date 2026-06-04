namespace MyTelegram.Domain.CommandHandlers.Temp;

public class StartForwardMessagesCommandHandler : CommandHandler<TempAggregate, TempId, StartForwardMessagesCommand>
{
    public override Task ExecuteAsync(TempAggregate aggregate, StartForwardMessagesCommand command, CancellationToken cancellationToken)
    {
        aggregate.StartForwardMessages(command.RequestInfo, command.Silent, command.Background, command.WithMyScore,
            command.DropAuthor, command.DropMediaCaptions, command.NoForwards, command.FromPeer, command.ToPeer,
            command.MessageIds, command.RandomIds, command.ScheduleDate, command.SendAs,
            command.ForwardFromLinkedChannel, command.Post);
        return Task.CompletedTask;
    }
}