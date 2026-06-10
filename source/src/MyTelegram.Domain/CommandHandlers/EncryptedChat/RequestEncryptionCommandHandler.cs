using MyTelegram.Domain.Aggregates.EncryptedChat;
using MyTelegram.Domain.Commands.EncryptedChat;

namespace MyTelegram.Domain.CommandHandlers.EncryptedChat;

public class RequestEncryptionCommandHandler : CommandHandler<EncryptedChatAggregate, EncryptedChatId, RequestEncryptionCommand>
{
    public override Task ExecuteAsync(EncryptedChatAggregate aggregate,
        RequestEncryptionCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.RequestEncryption(
            command.RequestInfo,
            command.ChatId,
            command.AccessHash,
            command.AdminId,
            command.ParticipantId,
            command.AdminPermAuthKeyId,
            command.GA,
            command.Date);

        return Task.CompletedTask;
    }
}
