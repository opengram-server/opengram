using MyTelegram.Domain.Shared.Business;
using MyTelegram.Messenger.Services.Impl;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Edit a business chat link.
/// See <a href="https://corefork.telegram.org/method/account.editBusinessChatLink" />
///</summary>
internal sealed class EditBusinessChatLinkHandler(
    IBusinessAppService businessAppService,
    IUserAppService userAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestEditBusinessChatLink, MyTelegram.Schema.IBusinessChatLink>,
        Account.IEditBusinessChatLinkHandler
{
    protected override async Task<MyTelegram.Schema.IBusinessChatLink> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestEditBusinessChatLink obj)
    {
        var userReadModel = await userAppService.GetAsync(input.UserId);
        if (userReadModel == null)
        {
            throw new RpcException(RpcErrors.RpcErrors400.UserIdInvalid);
        }

        if (!userReadModel.Premium && !userReadModel.Bot)
        {
            throw new RpcException(RpcErrors.RpcErrors403.PremiumAccountRequired);
        }

        if (string.IsNullOrEmpty(obj.Slug))
        {
            throw new RpcException(new RpcError(400, "SLUG_INVALID"));
        }

        // Find existing link
        var existingLinks = await businessAppService.GetBusinessChatLinksAsync(input.UserId);
        var existingLink = existingLinks.FirstOrDefault(l => l.Link.Contains(obj.Slug) || l.Id == obj.Slug);
        if (existingLink == null)
        {
            throw new RpcException(new RpcError(400, "SLUG_INVALID"));
        }

        // Delete old and create new (edit = delete + create with same slug)
        await businessAppService.DeleteBusinessChatLinkAsync(input.UserId, existingLink.Id);

        var updatedLink = await businessAppService.CreateBusinessChatLinkAsync(
            input.UserId,
            obj.Link.Title ?? existingLink.Title,
            obj.Link.Message ?? existingLink.Message,
            new List<string>());

        return new TBusinessChatLink
        {
            Link = updatedLink.Link,
            Title = updatedLink.Title,
            Message = updatedLink.Message,
            Entities = new TVector<IMessageEntity>(),
            Views = existingLink.Views
        };
    }
}
