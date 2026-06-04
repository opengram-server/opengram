// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Get <a href="https://corefork.telegram.org/api/reactions">message reaction</a> list, along with the sender of each reaction.
/// <para>Possible errors</para>
/// Code Type Description
/// 403 BROADCAST_FORBIDDEN Participants of polls in channels should stay anonymous.
/// 400 MSG_ID_INVALID Invalid message ID provided.
/// See <a href="https://corefork.telegram.org/method/messages.getMessageReactionsList" />
///</summary>
internal sealed class GetMessageReactionsListHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetMessageReactionsList, MyTelegram.Schema.Messages.IMessageReactionsList>,
    Messages.IGetMessageReactionsListHandler
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly ILayeredService<IUserConverter> _layeredService;

    public GetMessageReactionsListHandler(
        IQueryProcessor queryProcessor,
        ILayeredService<IUserConverter> layeredService)
    {
        _queryProcessor = queryProcessor;
        _layeredService = layeredService;
    }

    protected override async Task<MyTelegram.Schema.Messages.IMessageReactionsList> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetMessageReactionsList obj)
    {
        var peer = obj.Peer.ToPeer();
        
        // Get message
        var messageReadModel = await _queryProcessor.ProcessAsync(
            new GetMessageByIdQuery(MessageId.Create(peer.PeerId, obj.Id, false).Value),
            CancellationToken.None);

        if (messageReadModel == null)
        {
            RpcErrors.RpcErrors400.MsgIdInvalid.ThrowRpcError();
        }

        var reactions = new List<IMessagePeerReaction>();
        var userIds = new HashSet<long>();

        if (messageReadModel.RecentReactions2 != null)
        {
            // Filter by specific reaction if provided
            var filteredReactions = messageReadModel.RecentReactions2;
            
            if (obj.Reaction != null)
            {
                filteredReactions = filteredReactions
                    .Where(r => r.Reaction.GetReactionId() == obj.Reaction.GetReactionId())
                    .ToList();
            }

            // Apply pagination
            var offset = 0;
            if (!string.IsNullOrEmpty(obj.Offset))
            {
                int.TryParse(obj.Offset, out offset);
            }

            var paginatedReactions = filteredReactions
                .Skip(offset)
                .Take(obj.Limit)
                .ToList();

            foreach (var reaction in paginatedReactions)
            {
                reactions.Add(new TMessagePeerReaction
                {
                    Big = reaction.Big,
                    PeerId = reaction.PeerId.ToPeer(),
                    Date = reaction.Date,
                    Reaction = reaction.Reaction
                });

                if (reaction.PeerId.PeerType == PeerType.User)
                {
                    userIds.Add(reaction.PeerId.PeerId);
                }
            }
        }

        // Get users
        var users = new List<IUser>();
        if (userIds.Count > 0)
        {
            var userReadModels = await _queryProcessor.ProcessAsync(
                new GetUsersByIdListQuery(userIds.ToList()),
                CancellationToken.None);

            foreach (var userReadModel in userReadModels)
            {
                users.Add(_layeredService.GetConverter(input.Layer).ToUser(userReadModel));
            }
        }

        // Calculate next offset
        string? nextOffset = null;
        if (messageReadModel.RecentReactions2 != null)
        {
            var totalCount = obj.Reaction != null
                ? messageReadModel.RecentReactions2.Count(r => r.Reaction.GetReactionId() == obj.Reaction.GetReactionId())
                : messageReadModel.RecentReactions2.Count;

            var currentOffset = string.IsNullOrEmpty(obj.Offset) ? 0 : int.Parse(obj.Offset);
            if (currentOffset + obj.Limit < totalCount)
            {
                nextOffset = (currentOffset + obj.Limit).ToString();
            }
        }

        return new TMessageReactionsList
        {
            Count = reactions.Count,
            Reactions = new TVector<IMessagePeerReaction>(reactions),
            Chats = new TVector<IChat>(),
            Users = new TVector<IUser>(users),
            NextOffset = nextOffset
        };
    }
}
