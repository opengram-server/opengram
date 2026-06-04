namespace  MyTelegram.Handlers.Account.LayerN;

///<summary>
/// Install a theme
/// See <a href="https://corefork.telegram.org/method/account.installTheme" />
///</summary>
internal sealed class InstallThemeHandler(
    IHandlerHelper handlerHelper,
    IRequestConverter<MyTelegram.Schema.Account.LayerN.RequestInstallTheme,
        MyTelegram.Schema.Account.RequestInstallTheme> dataConverter)
    : ForwardRequestToNewHandler<
            MyTelegram.Schema.Account.LayerN.RequestInstallTheme,
            MyTelegram.Schema.Account.RequestInstallTheme
        >(handlerHelper, dataConverter),
        Account.LayerN.IInstallThemeHandler, IDistinctObjectHandler
{
}