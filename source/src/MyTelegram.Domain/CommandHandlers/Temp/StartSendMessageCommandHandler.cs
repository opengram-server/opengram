namespace MyTelegram.Domain.CommandHandlers.Temp;

public class
    StartSendMessageCommandHandler : CommandHandler<TempAggregate, TempId, StartSendMessageCommand>
{
    public override Task ExecuteAsync(TempAggregate aggregate, StartSendMessageCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.StartSendMessage(command.RequestInfo, command.SendMessageItems, command.IsSendQuickReplyMessages, command.IsSendGroupedMessages, command.ClearDraft);

        return Task.CompletedTask;
    }
}