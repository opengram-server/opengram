namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Help;

///<summary>
/// Can only be used by TSF members to obtain user information, see here for more info.
/// See <a href="https://corefork.telegram.org/method/help.getUserInfo" />
///</summary>
internal sealed class GetUserInfoHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Help.RequestGetUserInfo, MyTelegram.Schema.Help.IUserInfo>,
    Help.IGetUserInfoHandler
{
    protected override Task<MyTelegram.Schema.Help.IUserInfo> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Help.RequestGetUserInfo obj)
    {
        // This is a TSF-only method. On self-hosted, return empty info.
        return Task.FromResult<MyTelegram.Schema.Help.IUserInfo>(
            new MyTelegram.Schema.Help.TUserInfoEmpty());
    }
}
