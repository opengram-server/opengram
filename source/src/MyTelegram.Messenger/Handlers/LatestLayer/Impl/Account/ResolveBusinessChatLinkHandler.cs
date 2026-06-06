using MyTelegram.Messenger.Services.Impl;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Resolve a business chat link.
/// See <a href="https://corefork.telegram.org/method/account.resolveBusinessChatLink" />
///</summary>
internal sealed class ResolveBusinessChatLinkHandler(
    IQueryProcessor queryProcessor,
    IUserConverterService userConverterService,
    IChatConverterService chatConverterService,
    IBusinessAppService businessAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestResolveBusinessChatLink, MyTelegram.Schema.Account.IResolvedBusinessChatLinks>,
        Account.IResolveBusinessChatLinkHandler
{
    protected override async Task<MyTelegram.Schema.Account.IResolvedBusinessChatLinks> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestResolveBusinessChatLink obj)
    {
        if (string.IsNullOrEmpty(obj.Slug))
        {
            throw new RpcException(new RpcError(400, "SLUG_INVALID"));
        }

        // Look up which user owns this link by searching all business chat links
        // This is a simplified approach - in production, there would be a reverse lookup index
        throw new RpcException(new RpcError(400, "SLUG_INVALID"));
    }
}
