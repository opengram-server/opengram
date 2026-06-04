namespace MyTelegram.ReadModel.ReadModelLocators;

public class AccessHashId(string value) : Identity<AccessHashId>(value)
{
    public static AccessHashId Create(long id,
        long accessHash)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"{id}_{accessHash}");
    }
}