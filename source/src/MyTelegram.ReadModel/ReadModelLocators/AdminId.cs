using EventFlow.Core;

namespace MyTelegram.ReadModel.ReadModelLocators;

public class AdminId(string value) : Identity<AdminId>(value)
{
    public static AdminId Create(long peerId, long userId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"adminid-{peerId}-{userId}");
    }
}