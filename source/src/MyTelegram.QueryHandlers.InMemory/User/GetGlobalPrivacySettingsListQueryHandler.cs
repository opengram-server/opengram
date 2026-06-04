namespace MyTelegram.QueryHandlers.InMemory.User;

public class GetGlobalPrivacySettingsListQueryHandler(IQueryOnlyReadModelStore<UserReadModel> store) : IQueryHandler<GetGlobalPrivacySettingsListQuery,
    IReadOnlyDictionary<long, GlobalPrivacySettings?>>
{
    public async Task<IReadOnlyDictionary<long, GlobalPrivacySettings?>> ExecuteQueryAsync(GetGlobalPrivacySettingsListQuery query, CancellationToken cancellationToken)
    {
        var readModels = await store.FindAsync(p => query.UserIds.Contains(p.UserId), createResult: p => new GlobalPrivacySettingsWithUserId(p.UserId, p.GlobalPrivacySettings), cancellationToken: cancellationToken);

        return readModels.ToDictionary(k => k.UserId, v => v.GlobalPrivacySettings);
    }

    public record GlobalPrivacySettingsWithUserId(long UserId, GlobalPrivacySettings? GlobalPrivacySettings);
}

