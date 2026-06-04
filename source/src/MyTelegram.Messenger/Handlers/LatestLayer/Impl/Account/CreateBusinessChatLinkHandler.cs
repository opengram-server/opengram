using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Shared.Business;
using MyTelegram.Schema;
using MyTelegram.Schema.Account;
using MyTelegram.Handlers;
using MyTelegram.Services.Services;
using MyTelegram.Messenger.Services.Impl;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Create a <a href="https://corefork.telegram.org/api/business#business-chat-links">business chat deep link »</a>.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHATLINKS_TOO_MUCH Too many <a href="https://corefork.telegram.org/api/business#business-chat-links">business chat links</a> were created, please delete some older links.
/// 403 PREMIUM_ACCOUNT_REQUIRED A premium account is required to execute this action.
/// See <a href="https://corefork.telegram.org/method/account.createBusinessChatLink" />
///</summary>
internal sealed class CreateBusinessChatLinkHandler(
    ILogger<CreateBusinessChatLinkHandler> logger,
    IBusinessAppService businessAppService,
    IUserAppService userAppService) 
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestCreateBusinessChatLink, MyTelegram.Schema.IBusinessChatLink>,
    Account.ICreateBusinessChatLinkHandler
{
    protected override async Task<MyTelegram.Schema.IBusinessChatLink> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestCreateBusinessChatLink obj)
    {
        logger.LogInformation("Creating business chat link for user {UserId}", input.UserId);

        // Check if user has premium/business subscription
        var userReadModel = await userAppService.GetAsync(input.UserId);
        if (userReadModel == null)
        {
            throw new RpcException(RpcErrors.RpcErrors400.UserIdInvalid);
        }

        if (!userReadModel.Premium && !userReadModel.Bot)
        {
            throw new RpcException(RpcErrors.RpcErrors403.PremiumAccountRequired);
        }

        // Check if user exceeded the limit
        var existingLinks = await businessAppService.GetBusinessChatLinksAsync(input.UserId);
        const int maxBusinessChatLinks = 20; // Default limit, should be from config
        
        if (existingLinks.Count >= maxBusinessChatLinks)
        {
            throw new RpcException(new RpcError(400, "CHATLINKS_TOO_MUCH"));
        }

        // Validate link data
        if (string.IsNullOrEmpty(obj.Link.Title) || obj.Link.Title.Length > 32)
        {
            throw new RpcException(RpcErrors.RpcErrors400.TitleInvalid);
        }

        if (obj.Link.Message != null && obj.Link.Message.Length > 4096)
        {
            throw new RpcException(RpcErrors.RpcErrors400.TitleInvalid);
        }

        // Create the business chat link
        var messageEntities = obj.Link.Entities?.Select(e => ConvertToMessageEntity(e)).ToList() ?? new List<MyTelegram.Domain.Shared.Business.MessageEntity>();
        var entityStrings = messageEntities.Select(e => System.Text.Json.JsonSerializer.Serialize(e)).ToList();

        var businessChatLink = await businessAppService.CreateBusinessChatLinkAsync(
            input.UserId,
            obj.Link.Title,
            obj.Link.Message ?? string.Empty,
            entityStrings);

        logger.LogInformation("Business chat link created successfully for user {UserId}", 
            input.UserId);

        return new TBusinessChatLink
        {
            // Id = businessChatLink.Id,
            Link = businessChatLink.Link,
            Title = businessChatLink.Title,
            Message = businessChatLink.Message,
            Entities = new TVector<IMessageEntity>(businessChatLink.Entities.Select(e => CreateMessageEntity(e))),
            Views = businessChatLink.Views
        };
    }

    private static MyTelegram.Domain.Shared.Business.MessageEntity ConvertToMessageEntity(MyTelegram.Schema.IMessageEntity e)
    {
        // Convert Schema.IMessageEntity to Domain.Shared.Business.MessageEntity
        return new MyTelegram.Domain.Shared.Business.MessageEntity
        {
            Offset = e.Offset,
            Length = e.Length,
            Type = e switch
            {
                MyTelegram.Schema.TMessageEntityBold => "bold",
                MyTelegram.Schema.TMessageEntityItalic => "italic",
                MyTelegram.Schema.TMessageEntityCode => "code",
                MyTelegram.Schema.TMessageEntityPre => "pre",
                MyTelegram.Schema.TMessageEntityTextUrl => "text_url",
                MyTelegram.Schema.TMessageEntityUrl => "url",
                MyTelegram.Schema.TMessageEntityMention => "mention",
                MyTelegram.Schema.TMessageEntityHashtag => "hashtag",
                _ => "unknown"
            },
            Url = e is MyTelegram.Schema.TMessageEntityTextUrl textUrl ? textUrl.Url : null,
            Language = e is MyTelegram.Schema.TMessageEntityPre preEntity ? preEntity.Language : null
        };
    }
    
    private static IMessageEntity CreateMessageEntity(MyTelegram.Domain.Shared.Business.MessageEntity e)
    {
        // Convert Domain.Shared.Business.MessageEntity back to Schema.IMessageEntity
        return e.Type switch
        {
            "bold" => new MyTelegram.Schema.TMessageEntityBold { Offset = e.Offset, Length = e.Length },
            "italic" => new MyTelegram.Schema.TMessageEntityItalic { Offset = e.Offset, Length = e.Length },
            "code" => new MyTelegram.Schema.TMessageEntityCode { Offset = e.Offset, Length = e.Length },
            "pre" => new MyTelegram.Schema.TMessageEntityPre { Offset = e.Offset, Length = e.Length, Language = e.Language ?? string.Empty },
            "text_url" => new MyTelegram.Schema.TMessageEntityTextUrl { Offset = e.Offset, Length = e.Length, Url = e.Url ?? string.Empty },
            "url" => new MyTelegram.Schema.TMessageEntityUrl { Offset = e.Offset, Length = e.Length },
            "mention" => new MyTelegram.Schema.TMessageEntityMention { Offset = e.Offset, Length = e.Length },
            "hashtag" => new MyTelegram.Schema.TMessageEntityHashtag { Offset = e.Offset, Length = e.Length },
            _ => new MyTelegram.Schema.TMessageEntityUnknown { Offset = e.Offset, Length = e.Length }
        };
    }
}
