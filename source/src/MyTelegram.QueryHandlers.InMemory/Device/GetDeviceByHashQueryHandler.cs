namespace MyTelegram.QueryHandlers.InMemory.Device;

public class GetDeviceByHashQueryHandler(IQueryOnlyReadModelStore<DeviceReadModel> store) : IQueryHandler<GetDeviceByHashQuery, IDeviceReadModel?>
{
    public async Task<IDeviceReadModel?> ExecuteQueryAsync(GetDeviceByHashQuery query,
        CancellationToken cancellationToken)
    {
        return await store
            .FirstOrDefaultAsync(p => p.UserId == query.UserId && p.Hash == query.Hash, cancellationToken: cancellationToken);
    }
}
