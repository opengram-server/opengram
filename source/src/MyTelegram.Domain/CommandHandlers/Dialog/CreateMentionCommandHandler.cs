namespace MyTelegram.Domain.CommandHandlers.Dialog;

public class CreateMentionCommandHandler : CommandHandler<DialogAggregate, DialogId, CreateMentionCommand>
{
    public override Task ExecuteAsync(DialogAggregate aggregate, CreateMentionCommand command, CancellationToken cancellationToken)
    {
        aggregate.CreateMention(command.MessageId);

        return Task.CompletedTask;
    }
}