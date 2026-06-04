namespace MyTelegram.Domain.Aggregates.Language;

public class LanguageId(string value) : Identity<LanguageId>(value);

public class LanguageCreatedEvent : AggregateEvent<LanguageAggregate, LanguageId> { }

public class LanguageAggregate(LanguageId id) : AggregateRoot<LanguageAggregate, LanguageId>(id) { }

public class LanguageTextId(string value) : Identity<LanguageTextId>(value);

public class LanguageTextAggregate(LanguageTextId id) : AggregateRoot<LanguageTextAggregate, LanguageTextId>(id);
public class LanguageTextCreatedEvent : AggregateEvent<LanguageTextAggregate, LanguageTextId>;