namespace MyTelegram.QueryHandlers.InMemory.Device;

public class GetDeviceByUserIdQueryHandler(IQueryOnlyReadModelStore<DeviceReadModel> store) : IQueryHandler<GetDeviceByUserIdQuery, IReadOnlyCollection<IDeviceReadModel>>
{
    public async Task<IReadOnlyCollection<IDeviceReadModel>> ExecuteQueryAsync(GetDeviceByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.UserId == query.UserId && p.IsActive, cancellationToken: cancellationToken);
    }
}
