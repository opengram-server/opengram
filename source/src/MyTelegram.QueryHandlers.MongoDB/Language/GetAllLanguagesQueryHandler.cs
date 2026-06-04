namespace MyTelegram.QueryHandlers.MongoDB.Language;

public class GetAllLanguagesQueryHandler(IQueryOnlyReadModelStore<LanguageReadModel> store) : IQueryHandler<GetAllLanguagesQuery, IReadOnlyCollection<ILanguageReadModel>>
{
    public async Task<IReadOnlyCollection<ILanguageReadModel>> ExecuteQueryAsync(GetAllLanguagesQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.IsEnabled, cancellationToken: cancellationToken);
    }
}