namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Users;

///<summary>
/// Check whether we can write to the specified user (this method can only be called by non-<a href="https://corefork.telegram.org/api/premium">Premium</a> users), see <a href="https://corefork.telegram.org/api/privacy#require-premium-for-new-non-contact-users">here »</a> for more info on the full flow.
/// See <a href="https://corefork.telegram.org/method/users.getIsPremiumRequiredToContact" />
///</summary>
internal sealed class GetIsPremiumRequiredToContactHandler(
    IQueryProcessor queryProcessor,
    IAccessHashHelper accessHashHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Users.RequestGetIsPremiumRequiredToContact, TVector<bool>>,
        Users.IGetIsPremiumRequiredToContactHandler
{
    protected override async Task<TVector<bool>> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Users.RequestGetIsPremiumRequiredToContact obj)
    {
        var userIds = new List<long>();
        foreach (var item in obj.Id)
        {
            if (item is TInputUser inputUser)
            {
                await accessHashHelper.CheckAccessHashAsync(input, inputUser);
                userIds.Add(inputUser.UserId);
            }
        }

        var r = await queryProcessor.ProcessAsync(new GetGlobalPrivacySettingsListQuery(userIds));

        var result = new TVector<bool>();
        foreach (var item in obj.Id)
        {
            if (item is TInputUser inputUser)
            {
                if (r.TryGetValue(inputUser.UserId, out var globalPrivacySettings))
                {
                    result.Add(globalPrivacySettings?.NewNoncontactPeersRequirePremium ?? false);
                }
                else
                {
                    result.Add(false);
                }
            }
            else
            {
                result.Add(false);
            }
        }

        return result;
    }
}
