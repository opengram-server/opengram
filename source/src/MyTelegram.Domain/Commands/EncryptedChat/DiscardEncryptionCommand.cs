using MyTelegram.Domain.Aggregates.EncryptedChat;

namespace MyTelegram.Domain.Commands.EncryptedChat;

public class DiscardEncryptionCommand(
    EncryptedChatId aggregateId,
    RequestInfo requestInfo,
    bool deleteHistory)
    : RequestCommand2<EncryptedChatAggregate, EncryptedChatId, IExecutionResult>(aggregateId, requestInfo)
{
    public bool DeleteHistory { get; } = deleteHistory;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(DeleteHistory);
    }
}
