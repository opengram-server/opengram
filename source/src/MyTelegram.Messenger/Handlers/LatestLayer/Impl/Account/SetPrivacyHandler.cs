// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Change privacy settings of current account
/// <para>Possible errors</para>
/// Code Type Description
/// 400 PRIVACY_KEY_INVALID The privacy key is invalid.
/// 400 PRIVACY_TOO_LONG Too many privacy rules were specified, the current limit is 1000.
/// 400 PRIVACY_VALUE_INVALID The specified privacy rule combination is invalid.
/// See <a href="https://corefork.telegram.org/method/account.setPrivacy" />
///</summary>
internal sealed class SetPrivacyHandler(
    IPrivacyAppService privacyAppService,
    ICommandBus commandBus) 
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestSetPrivacy, MyTelegram.Schema.Account.IPrivacyRules>,
    Account.ISetPrivacyHandler
{
    protected override async Task<MyTelegram.Schema.Account.IPrivacyRules> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestSetPrivacy obj)
    {
        // Pass full IRequestInput to preserve ReqMsgId for command deduplication
        await privacyAppService.SetPrivacyAsync(
            input,
            input.UserId,
            obj.Key,
            obj.Rules.ToList());

        var rules = await privacyAppService.GetPrivacyRulesAsync(input.UserId, obj.Key);

        return new TPrivacyRules
        {
            Rules = rules,
            Chats = new TVector<IChat>(),
            Users = new TVector<IUser>()
        };
    }
}
