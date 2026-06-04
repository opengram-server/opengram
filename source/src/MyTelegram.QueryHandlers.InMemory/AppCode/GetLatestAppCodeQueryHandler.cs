namespace MyTelegram.QueryHandlers.InMemory.AppCode;

public class GetLatestAppCodeQueryHandler(IQueryOnlyReadModelStore<AppCodeReadModel> store) : IQueryHandler<GetLatestAppCodeQuery, IAppCodeReadModel?>
{
    public async Task<IAppCodeReadModel?> ExecuteQueryAsync(GetLatestAppCodeQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p =>
            p.PhoneNumber == query.PhoneNumber && p.PhoneCodeHash == query.PhoneCodeHash, cancellationToken);
    }
}
