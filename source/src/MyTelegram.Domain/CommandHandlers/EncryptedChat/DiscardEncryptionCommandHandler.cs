using MyTelegram.Domain.Aggregates.EncryptedChat;
using MyTelegram.Domain.Commands.EncryptedChat;

namespace MyTelegram.Domain.CommandHandlers.EncryptedChat;

public class DiscardEncryptionCommandHandler : CommandHandler<EncryptedChatAggregate, EncryptedChatId, DiscardEncryptionCommand>
{
    public override Task ExecuteAsync(EncryptedChatAggregate aggregate,
        DiscardEncryptionCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.DiscardEncryption(
            command.RequestInfo,
            command.DeleteHistory);

        return Task.CompletedTask;
    }
}
