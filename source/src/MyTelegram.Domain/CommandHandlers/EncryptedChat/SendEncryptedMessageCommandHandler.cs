using MyTelegram.Domain.Aggregates.EncryptedChat;
using MyTelegram.Domain.Commands.EncryptedChat;

namespace MyTelegram.Domain.CommandHandlers.EncryptedChat;

public class SendEncryptedMessageCommandHandler : CommandHandler<EncryptedChatAggregate, EncryptedChatId, SendEncryptedMessageCommand>
{
    public override Task ExecuteAsync(EncryptedChatAggregate aggregate,
        SendEncryptedMessageCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.SendEncryptedMessage(
            command.RequestInfo,
            command.RandomId,
            command.Data,
            command.FileData,
            command.MessageType,
            command.Date);

        return Task.CompletedTask;
    }
}
