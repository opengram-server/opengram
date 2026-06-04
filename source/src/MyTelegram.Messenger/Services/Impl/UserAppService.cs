namespace MyTelegram.Messenger.Services.Impl;

public class UserAppService(IQueryProcessor queryProcessor,
    IReadModelCacheHelper<IUserReadModel> userReadModelCacheHelper,
    IReadModelCacheHelper<IUserFullReadModel> userFullReadModelCacheHelper) : ReadModelWithCacheAppService<IUserReadModel>(userReadModelCacheHelper), IUserAppService, ITransientDependency
{
    public async Task CheckAccountPremiumStatusAsync(long userId)
    {
        var userReadModel = await queryProcessor.ProcessAsync<IUserReadModel>(new GetUserByIdQuery(userId), CancellationToken.None);
        if (userReadModel == null)
        {
            RpcErrors.RpcErrors400.UserIdInvalid.ThrowRpcError();
        }

        if (!userReadModel!.Premium)
        {
            RpcErrors.RpcErrors400.PremiumAccountRequired.ThrowRpcError();
        }
    }


    public Task<IUserFullReadModel?> GetUserFullAsync(long userId)
    {
        return userFullReadModelCacheHelper.GetOrCreateAsync(userId,
            () => queryProcessor.ProcessAsync<IUserFullReadModel>(new GetUserFullQuery(userId), CancellationToken.None), p => p.Id);
    }

    protected override async Task<IUserReadModel?> GetReadModelAsync(long id)
    {
        var userReadModel = await queryProcessor.ProcessAsync<IUserReadModel>(new GetUserByIdQuery(id), CancellationToken.None);

        if (userReadModel == null)
        {
            return await CreateNonExistsReadModelAsync(id);
        }

        return userReadModel;
    }

    protected override string GetReadModelId(IUserReadModel readModel) => readModel.Id;

    protected override long GetReadModelInt64Id(IUserReadModel readModel) => readModel.UserId;
    protected override Task<IUserReadModel?> CreateNonExistsReadModelAsync(long id)
    {
        return Task.FromResult<IUserReadModel?>(new UserReadModel { UserId = id, IsDeleted = true, Id = UserId.Create(id).Value });
    }

    protected override Task<IReadOnlyCollection<IUserReadModel>> GetReadModelListAsync(List<long> ids)
    {
        return queryProcessor.ProcessAsync<IReadOnlyCollection<IUserReadModel>>(new GetUsersByUserIdListQuery(ids), CancellationToken.None);
    }
}