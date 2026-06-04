using EventFlow.Core;

namespace MyTelegram.ReadModel.ReadModelLocators;

public class PtsForAuthKeyId(string value) : Identity<PtsForAuthKeyId>(value)
{
    public static PtsForAuthKeyId Create(long ownerPeerId, long permAuthKeyId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands,
            $"ptsforauthkeyidreadmodel-{ownerPeerId}-{permAuthKeyId}");
    }
}