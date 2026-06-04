namespace MyTelegram.QueryHandlers.InMemory.Language;

public class
    GetAllLanguageTextsQueryHandler(IQueryOnlyReadModelStore<LanguageTextReadModel> store) : IQueryHandler<GetAllLanguageTextsQuery,
    IReadOnlyCollection<ILanguageTextReadModel>>
{
    public async Task<IReadOnlyCollection<ILanguageTextReadModel>> ExecuteQueryAsync(GetAllLanguageTextsQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => true, cancellationToken: cancellationToken);
    }
}
