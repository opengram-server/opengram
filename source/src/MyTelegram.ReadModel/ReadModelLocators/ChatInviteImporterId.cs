using EventFlow.Core;

namespace MyTelegram.ReadModel.ReadModelLocators;

public class ChatInviteImporterId(string value) : Identity<ChatInviteImporterId>(value)
{
    public static ChatInviteImporterId Create(long peerId, long userId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"chatinviteimporter-{peerId}-{userId}");
    }
}