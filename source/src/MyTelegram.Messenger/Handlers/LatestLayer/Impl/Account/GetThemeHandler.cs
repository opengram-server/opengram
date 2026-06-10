namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Get theme information.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 THEME_INVALID The specified theme is invalid.
/// See <a href="https://corefork.telegram.org/method/account.getTheme" />
///</summary>
internal sealed class GetThemeHandler(
    IQueryProcessor queryProcessor)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestGetTheme, MyTelegram.Schema.ITheme>,
        Account.IGetThemeHandler
{
    protected override async Task<ITheme> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestGetTheme obj)
    {
        // Themes infrastructure is limited - return a default theme
        throw new RpcException(new RpcError(400, "THEME_INVALID"));
    }
}
