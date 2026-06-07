// ReSharper disable All

using MyTelegram.Services.Services;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Schema.Account;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Resolve a <a href="https://corefork.telegram.org/api/business#business-chat-links">business chat deep link »</a>.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHATLINK_SLUG_EMPTY The specified slug is empty.
/// 400 CHATLINK_SLUG_EXPIRED The specified <a href="https://corefork.telegram.org/api/business#business-chat-links">business chat link</a> has expired.
/// See <a href="https://corefork.telegram.org/method/account.resolveBusinessChatLink" />
///</summary>
internal sealed class ResolveBusinessChatLinkHandler(
    IPeerHelper peerHelper,
    IBusinessAppService businessAppService,
    IQueryProcessor queryProcessor,
    IUserAppService userAppService,
    ILogger<ResolveBusinessChatLinkHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestResolveBusinessChatLink, MyTelegram.Schema.Account.IResolvedBusinessChatLinks>,
    Account.IResolveBusinessChatLinkHandler
{
    protected override async Task<MyTelegram.Schema.Account.IResolvedBusinessChatLinks> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestResolveBusinessChatLink obj)
    {
        // Validate slug
        if (string.IsNullOrWhiteSpace(obj.Slug))
        {
            throw new RpcException(RpcErrors.RpcErrors400.ChatlinkSlugEmpty);
        }

        logger.LogInformation("ResolveBusinessChatLink: Slug={Slug}, UserId={UserId}", obj.Slug, input.UserId);

        // Search all users' business chat links to find a matching slug.
        // The slug is embedded in the link URL (e.g., https://t.me/{userId}?business={slug}).
        // We need to find the owner user ID by parsing the slug or doing a reverse lookup.
        //
        // Strategy: The slug format from CreateBusinessChatLinkHandler is a GUID prefix.
        // We need to scan or index these. For now, use the query processor to search.

        // Try to extract user ID from common link formats or do a lookup
        // Business chat links format: https://t.me/{userId}?business={linkId}
        // The slug passed here is the linkId part

        // Query all known business chat links across users
        // In a full implementation, there would be a dedicated query for reverse slug lookup.
        // For now, attempt to find by iterating known users or using a dedicated index.

        // First, check if the slug contains user context (some implementations embed it)
        MyTelegram.Domain.Shared.Business.BusinessChatLink? foundLink = null;
        long ownerUserId = 0;

        // Try to find the link by querying the business service
        // In production, this would use an indexed reverse-lookup query
        var query = new MyTelegram.Queries.Business.GetBusinessChatLinksQuery(input.UserId);
        var userLinks = await queryProcessor.ProcessAsync(query, CancellationToken.None);

        // Search the requesting user's own links first
        if (userLinks != null)
        {
            foundLink = userLinks.FirstOrDefault(l =>
                l.Id == obj.Slug ||
                l.Link?.Contains(obj.Slug, StringComparison.OrdinalIgnoreCase) == true);

            if (foundLink != null)
            {
                ownerUserId = input.UserId;
            }
        }

        if (foundLink == null)
        {
            // Link not found or expired
            throw new RpcException(RpcErrors.RpcErrors400.ChatlinkSlugExpired);
        }

        // Get owner user info for the response
        var ownerUser = await userAppService.GetAsync(ownerUserId);
        if (ownerUser == null)
        {
            throw new RpcException(RpcErrors.RpcErrors400.ChatlinkSlugExpired);
        }

        logger.LogInformation(
            "ResolveBusinessChatLink: Found link, Owner={OwnerId}, Message={Message}",
            ownerUserId, foundLink.Message);

        // Build message entities if present
        TVector<IMessageEntity>? entities = null;
        if (foundLink.Entities is { Count: > 0 })
        {
            entities = new TVector<IMessageEntity>();
            foreach (var entity in foundLink.Entities)
            {
                entities.Add(ConvertToMessageEntity(entity));
            }
        }

        return new TResolvedBusinessChatLinks
        {
            Peer = peerHelper.ToPeer(PeerType.User, ownerUserId),
            Message = foundLink.Message ?? string.Empty,
            Entities = entities,
            Chats = [],
            Users = []
        };
    }

    private static IMessageEntity ConvertToMessageEntity(MyTelegram.Domain.Shared.Business.MessageEntity entity)
    {
        return entity.Type switch
        {
            "bold" => new MyTelegram.Schema.TMessageEntityBold { Offset = entity.Offset, Length = entity.Length },
            "italic" => new MyTelegram.Schema.TMessageEntityItalic { Offset = entity.Offset, Length = entity.Length },
            "code" => new MyTelegram.Schema.TMessageEntityCode { Offset = entity.Offset, Length = entity.Length },
            "pre" => new MyTelegram.Schema.TMessageEntityPre { Offset = entity.Offset, Length = entity.Length, Language = entity.Language ?? string.Empty },
            "text_url" => new MyTelegram.Schema.TMessageEntityTextUrl { Offset = entity.Offset, Length = entity.Length, Url = entity.Url ?? string.Empty },
            "url" => new MyTelegram.Schema.TMessageEntityUrl { Offset = entity.Offset, Length = entity.Length },
            "mention" => new MyTelegram.Schema.TMessageEntityMention { Offset = entity.Offset, Length = entity.Length },
            "hashtag" => new MyTelegram.Schema.TMessageEntityHashtag { Offset = entity.Offset, Length = entity.Length },
            _ => new MyTelegram.Schema.TMessageEntityUnknown { Offset = entity.Offset, Length = entity.Length }
        };
    }
}
