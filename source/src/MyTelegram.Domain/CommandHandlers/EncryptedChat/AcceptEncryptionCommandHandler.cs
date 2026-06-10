using MyTelegram.Domain.Aggregates.EncryptedChat;
using MyTelegram.Domain.Commands.EncryptedChat;

namespace MyTelegram.Domain.CommandHandlers.EncryptedChat;

public class AcceptEncryptionCommandHandler : CommandHandler<EncryptedChatAggregate, EncryptedChatId, AcceptEncryptionCommand>
{
    public override Task ExecuteAsync(EncryptedChatAggregate aggregate,
        AcceptEncryptionCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.AcceptEncryption(
            command.RequestInfo,
            command.GB,
            command.KeyFingerprint,
            command.ParticipantPermAuthKeyId);

        return Task.CompletedTask;
    }
}
