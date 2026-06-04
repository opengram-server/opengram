namespace MyTelegram.Domain.Aggregates.Document;
public class DocumentId(string value) : Identity<DocumentId>(value)
{
    public static DocumentId Create(long serverFileId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"documentid-{serverFileId}");
    }
}