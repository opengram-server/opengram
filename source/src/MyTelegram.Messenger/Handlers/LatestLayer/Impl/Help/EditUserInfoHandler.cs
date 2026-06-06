namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Help;

///<summary>
/// Can only be used by TSF members to edit user info, see here for more info.
/// See <a href="https://corefork.telegram.org/method/help.editUserInfo" />
///</summary>
internal sealed class EditUserInfoHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Help.RequestEditUserInfo, MyTelegram.Schema.Help.IUserInfo>,
    Help.IEditUserInfoHandler
{
    protected override Task<MyTelegram.Schema.Help.IUserInfo> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Help.RequestEditUserInfo obj)
    {
        // This is a TSF-only method. On self-hosted, return empty info.
        return Task.FromResult<MyTelegram.Schema.Help.IUserInfo>(
            new MyTelegram.Schema.Help.TUserInfoEmpty());
    }
}
