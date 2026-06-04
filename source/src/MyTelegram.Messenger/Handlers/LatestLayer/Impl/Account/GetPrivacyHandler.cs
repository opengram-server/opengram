// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Get privacy settings of current account
/// <para>Possible errors</para>
/// Code Type Description
/// 400 PRIVACY_KEY_INVALID The privacy key is invalid.
/// See <a href="https://corefork.telegram.org/method/account.getPrivacy" />
///</summary>
internal sealed class GetPrivacyHandler(IPrivacyAppService privacyAppService) 
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestGetPrivacy, MyTelegram.Schema.Account.IPrivacyRules>,
    Account.IGetPrivacyHandler
{
    protected override async Task<MyTelegram.Schema.Account.IPrivacyRules> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestGetPrivacy obj)
    {
        var rules = await privacyAppService.GetPrivacyRulesAsync(input.UserId, obj.Key);
        
        return new TPrivacyRules
        {
            Chats = new TVector<IChat>(),
            Rules = rules,
            Users = new TVector<IUser>()
        };
    }
}
