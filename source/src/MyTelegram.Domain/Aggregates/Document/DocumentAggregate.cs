namespace MyTelegram.Domain.Aggregates.Document;

public class DocumentAggregate(DocumentId id) : AggregateRoot<DocumentAggregate, DocumentId>(id);