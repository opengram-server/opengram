// ReSharper disable All

using EventFlow.Exceptions;
using Microsoft.Extensions.Logging;
using MyTelegram.Schema.Extensions;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// React to message.Starting from layer 159, the reaction will be sent from the peer specified using <a href="https://corefork.telegram.org/method/messages.saveDefaultSendAs">messages.saveDefaultSendAs</a>.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHANNEL_INVALID The provided channel is invalid.
/// 400 CHANNEL_PRIVATE You haven't joined this channel/supergroup.
/// 403 CHAT_WRITE_FORBIDDEN You can't write in this chat.
/// 400 MESSAGE_ID_INVALID The provided message id is invalid.
/// 400 MESSAGE_NOT_MODIFIED The provided message data is identical to the previous message data, the message wasn't modified.
/// 400 MSG_ID_INVALID Invalid message ID provided.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// 403 PREMIUM_ACCOUNT_REQUIRED A premium account is required to execute this action.
/// 400 REACTIONS_TOO_MANY The message already has exactly <code>reactions_uniq_max</code> reaction emojis, you can't react with a new emoji, see <a href="https://corefork.telegram.org/api/config#client-configuration">the docs for more info »</a>.
/// 400 REACTION_EMPTY Empty reaction provided.
/// 400 REACTION_INVALID The specified reaction is invalid.
/// 400 USER_BANNED_IN_CHANNEL You're banned from sending messages in supergroups/channels.
/// See <a href="https://corefork.telegram.org/method/messages.sendReaction" />
///</summary>
internal sealed class SendReactionHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSendReaction, MyTelegram.Schema.IUpdates>,
    Messages.ISendReactionHandler
{
    private readonly ICommandBus _commandBus;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IPtsHelper _ptsHelper;
    private readonly ILayeredService<IUserConverter> _layeredService;
    private readonly ILogger<SendReactionHandler> _logger;

    public SendReactionHandler(
        ICommandBus commandBus,
        IQueryProcessor queryProcessor,
        IPtsHelper ptsHelper,
        ILayeredService<IUserConverter> layeredService,
        ILogger<SendReactionHandler> logger)
    {
        _commandBus = commandBus;
        _queryProcessor = queryProcessor;
        _ptsHelper = ptsHelper;
        _layeredService = layeredService;
        _logger = logger;
    }

    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestSendReaction obj)
    {
        var peer = obj.Peer.ToPeer();
        var selfUserId = input.UserId;
        if (peer.PeerType == PeerType.Self)
        {
            peer = new Peer(PeerType.User, selfUserId);
        }

        var date = DateTime.UtcNow.ToTimestamp();
        
        _logger.LogInformation(
            "*** SendReaction: PeerType={PeerType}, PeerId={PeerId}, MsgId={MsgId}, Big={Big}",
            peer.PeerType, peer.PeerId, obj.MsgId, obj.Big);

        // Get message
        var messageReadModel = await _queryProcessor.ProcessAsync(
            new GetMessageByIdQuery(MessageId.Create(peer.PeerId, obj.MsgId, false).Value),
            CancellationToken.None);

        if (messageReadModel == null)
        {
            RpcErrors.RpcErrors400.MessageIdInvalid.ThrowRpcError();
        }

        var senderPeer = new Peer(PeerType.User, selfUserId);

        // Handle reaction: if empty list or null - remove all reactions, otherwise add/update
        if (obj.Reaction == null || obj.Reaction.Count == 0)
        {
            // Remove only THIS USER's reactions (not all reactions!)
            if (messageReadModel.RecentReactions2 != null)
            {
                var userReactions = messageReadModel.RecentReactions2
                    .Where(r => r.PeerId.PeerId == selfUserId)
                    .ToList();

                foreach (var userReaction in userReactions)
                {
                    try
                    {
                        var command = new RemoveReactionCommand(
                            MessageId.Create(peer.PeerId, obj.MsgId),
                            input.ToRequestInfo(),
                            selfUserId,
                            userReaction.Reaction);

                        await _commandBus.PublishAsync(command, CancellationToken.None);
                    }
                    catch (DuplicateOperationException)
                    {
                        // Ignore duplicate commands - happens when client sends same request multiple times
                        // The operation was already performed, so we can safely continue
                    }
                }
            }
        }
        else
        {
            // Add reactions (replace old ones)
            // First remove all existing reactions from this user
            if (messageReadModel.RecentReactions2 != null)
            {
                var userReactions = messageReadModel.RecentReactions2
                    .Where(r => r.PeerId.PeerId == selfUserId)
                    .ToList();

                foreach (var userReaction in userReactions)
                {
                    try
                    {
                        var removeCommand = new RemoveReactionCommand(
                            MessageId.Create(peer.PeerId, obj.MsgId),
                            input.ToRequestInfo(),
                            selfUserId,
                            userReaction.Reaction);

                        await _commandBus.PublishAsync(removeCommand, CancellationToken.None);
                    }
                    catch (DuplicateOperationException)
                    {
                        // Ignore duplicate commands
                    }
                }
            }

            // Add new reactions
            foreach (var reaction in obj.Reaction)
            {
                // Validate custom emoji reactions (premium check)
                if (reaction is TReactionCustomEmoji)
                {
                    // TODO: Check if user has premium
                    // For now, allow all custom emoji reactions
                }

                try
                {
                    // For channels, reactions should always be small (big: false)
                    // For private chats and groups, use client's preference
                    var isBig = peer.PeerType == PeerType.Channel ? false : obj.Big;
                    
                    var command = new AddReactionCommand(
                        MessageId.Create(peer.PeerId, obj.MsgId),
                        input.ToRequestInfo(),
                        selfUserId,
                        senderPeer,
                        reaction,
                        isBig,
                        obj.AddToRecent,
                        date);

                    await _commandBus.PublishAsync(command, CancellationToken.None);
                }
                catch (DuplicateOperationException)
                {
                    // Ignore duplicate commands
                }
            }
        }

        // Update messageReadModel in memory to avoid stale read
        var updatedMessage = messageReadModel;
        updatedMessage.Reactions ??= new List<ReactionCount>();
        updatedMessage.RecentReactions2 ??= new List<MessagePeerReaction>();

        if (obj.Reaction == null || obj.Reaction.Count == 0)
        {
            // Remove only THIS USER's reactions (not all reactions!)
            if (updatedMessage.RecentReactions2 != null)
            {
                var userReactions = updatedMessage.RecentReactions2
                    .Where(r => r.PeerId.PeerId == selfUserId)
                    .ToList();

                foreach (var userReaction in userReactions)
                {
                    // Remove from Reactions count
                    var reactionCount = updatedMessage.Reactions.FirstOrDefault(r => r.GetReactionId() == userReaction.Reaction.GetReactionId());
                    if (reactionCount != null)
                    {
                        reactionCount.Count--;
                        if (reactionCount.Count <= 0)
                        {
                            updatedMessage.Reactions.Remove(reactionCount);
                        }
                    }
                    
                    // Remove from RecentReactions2
                    updatedMessage.RecentReactions2.Remove(userReaction);
                }
            }
        }
        else
        {
            // Add reactions (replace old ones)
            // First remove all existing reactions from this user
            if (updatedMessage.RecentReactions2 != null)
            {
                var userReactions = updatedMessage.RecentReactions2
                    .Where(r => r.PeerId.PeerId == selfUserId)
                    .ToList();

                foreach (var userReaction in userReactions)
                {
                    // Remove from Reactions count
                    var reactionCount = updatedMessage.Reactions.FirstOrDefault(r => r.GetReactionId() == userReaction.Reaction.GetReactionId());
                    if (reactionCount != null)
                    {
                        reactionCount.Count--;
                        if (reactionCount.Count <= 0)
                        {
                            updatedMessage.Reactions.Remove(reactionCount);
                        }
                    }
                    
                    // Remove from RecentReactions2
                    updatedMessage.RecentReactions2.Remove(userReaction);
                }
            }

            // Add new reactions
            foreach (var reaction in obj.Reaction)
            {
                // Find or create reaction count
                var reactionCount = updatedMessage.Reactions.FirstOrDefault(r => r.GetReactionId() == reaction.GetReactionId());
                if (reactionCount == null)
                {
                    string? emoticon = null;
                    long? customEmojiDocumentId = null;
                    
                    if (reaction is TReactionEmoji emojiReaction)
                    {
                        emoticon = emojiReaction.Emoticon;
                    }
                    else if (reaction is TReactionCustomEmoji customReaction)
                    {
                        customEmojiDocumentId = customReaction.DocumentId;
                    }

                    reactionCount = new ReactionCount(reaction, 1, emoticon, customEmojiDocumentId);
                    updatedMessage.Reactions.Add(reactionCount);
                }
                else
                {
                    reactionCount.Count++;
                }

                // Add to recent reactions
                var peerReaction = new MessagePeerReaction
                {
                    Big = obj.Big,
                    PeerId = new Peer(PeerType.User, selfUserId),
                    SenderUserId = selfUserId,
                    Date = date,
                    Reaction = reaction
                };
                
                updatedMessage.RecentReactions2.Insert(0, peerReaction);
            }
        }

        // Create update
        var pts = _ptsHelper.GetCachedPts(peer.PeerId);
        var update = new TUpdateMessageReactions
        {
            Peer = peer.ToPeer(),
            MsgId = obj.MsgId,
            Reactions = CreateMessageReactions(updatedMessage, selfUserId)
        };

        return new TUpdates
        {
            Updates = new TVector<IUpdate>(update),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = date,
            Seq = 0
        };
    }

    private IMessageReactions CreateMessageReactions(IMessageReadModel? message, long selfUserId)
    {
        if (message?.Reactions == null || message.Reactions.Count == 0)
        {
            return new TMessageReactions
            {
                Results = new TVector<IReactionCount>(),
                CanSeeList = false
            };
        }

        var results = message.Reactions.Select(r => new TReactionCount
        {
            Reaction = r.Reaction,
            Count = r.Count
        }).ToList();

        // For channels: don't show who reacted (anonymous reactions)
        // For groups/private chats: show recent reactions with users
        var isChannel = message.ToPeerType == PeerType.Channel;
        
        // Check if current user has reacted
        if (message.RecentReactions2 != null)
        {
            foreach (var result in results)
            {
                // Simple check: if any recent reaction matches selfUserId and emoji
                // Note: Reaction comparison might need to be more robust for custom emojis
                var userReaction = message.RecentReactions2.FirstOrDefault(p =>
                    p.PeerId.PeerId == selfUserId && p.Reaction.GetReactionId() == result.Reaction.GetReactionId());
                
                if (userReaction != null)
                {
                    result.ChosenOrder = 1; // Set to any positive integer to indicate "chosen"
                }
            }
        }
        
        _logger.LogInformation("*** CreateMessageReactions: ToPeerType={ToPeerType}, IsChannel={IsChannel}, RecentReactionsCount={Count}",
            message.ToPeerType, isChannel, message.RecentReactions2?.Count ?? 0);
        
        var recentReactions = !isChannel && message.RecentReactions2 != null
            ? message.RecentReactions2.Select(r => new TMessagePeerReaction
            {
                Big = r.Big,
                PeerId = r.PeerId.ToPeer(),
                Date = r.Date,
                Reaction = r.Reaction
            }).ToList()
            : null;

        return new TMessageReactions
        {
            Results = new TVector<IReactionCount>(results),
            RecentReactions = recentReactions != null ? new TVector<IMessagePeerReaction>(recentReactions) : null,
            CanSeeList = !isChannel && message.Reactions?.Any() == true
        };
    }
}
