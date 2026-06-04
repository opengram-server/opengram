namespace MyTelegram.QueryHandlers.InMemory.Device;

public class GetDeviceByAuthKeyIdQueryHandler(IQueryOnlyReadModelStore<DeviceReadModel> store) : IQueryHandler<GetDeviceByAuthKeyIdQuery, IDeviceReadModel?>
{
    public async Task<IDeviceReadModel?> ExecuteQueryAsync(GetDeviceByAuthKeyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.PermAuthKeyId == query.AuthKeyId, cancellationToken);
    }
}
