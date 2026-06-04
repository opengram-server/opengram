// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Obtain all chat themes associated to owned collectible gifts
/// See <a href="https://corefork.telegram.org/method/account.getUniqueGiftChatThemes" />
///</summary>
internal sealed class GetUniqueGiftChatThemesHandler(
    ILogger<GetUniqueGiftChatThemesHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestGetUniqueGiftChatThemes, MyTelegram.Schema.Account.IChatThemes>,
    Interfaces.Account.IGetUniqueGiftChatThemesHandler
{
    protected override Task<MyTelegram.Schema.Account.IChatThemes> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestGetUniqueGiftChatThemes obj)
    {
        logger.LogInformation("GetUniqueGiftChatThemesHandler: User {UserId} requested unique gift chat themes (offset={Offset}, limit={Limit}, hash={Hash})", 
            input.UserId, obj.Offset, obj.Limit, obj.Hash);
        
        // For now, return empty list since we don't support collectible gifts yet
        // In the future, this should query user's owned collectible gifts and return associated themes
        var result = new TAccountChatThemes
        {
            Hash = 0,
            Themes = new TVector<IChatTheme>(),
            Chats = new TVector<IChat>(),
            Users = new TVector<IUser>()
        };
        
        logger.LogInformation("GetUniqueGiftChatThemesHandler: Returning empty list (collectible gifts not implemented)");
        
        return Task.FromResult<MyTelegram.Schema.Account.IChatThemes>(result);
    }
}
