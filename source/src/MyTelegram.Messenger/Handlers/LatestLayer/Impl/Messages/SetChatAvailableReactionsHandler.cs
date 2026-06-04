// ReSharper disable All

using MyTelegram.Domain.Aggregates.Chat;
using MyTelegram.Domain.Commands.Chat;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Change the set of <a href="https://corefork.telegram.org/api/reactions">message reactions »</a> that can be used in a certain group, supergroup or channel
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHAT_ADMIN_REQUIRED You must be an admin in this chat to do this.
/// 400 CHAT_NOT_MODIFIED No changes were made to chat information because the new information you passed is identical to the current information.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// See <a href="https://corefork.telegram.org/method/messages.setChatAvailableReactions" />
///</summary>
internal sealed class SetChatAvailableReactionsHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSetChatAvailableReactions, MyTelegram.Schema.IUpdates>,
    Messages.ISetChatAvailableReactionsHandler
{
    private readonly ICommandBus _commandBus;

    public SetChatAvailableReactionsHandler(ICommandBus commandBus)
    {
        _commandBus = commandBus;
    }

    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestSetChatAvailableReactions obj)
    {
        var peer = obj.Peer.ToPeer();
        var reactionType = ReactionType.ReactionNone;
        var allowCustomReaction = false;
        List<string>? availableReactions = null;

        switch (obj.AvailableReactions)
        {
            case TChatReactionsNone:
                reactionType = ReactionType.ReactionNone;
                break;

            case TChatReactionsAll all:
                reactionType = ReactionType.ReactionAll;
                allowCustomReaction = all.AllowCustom;
                break;

            case TChatReactionsSome some:
                reactionType = ReactionType.ReactionSome;
                availableReactions = new List<string>();
                
                foreach (var reaction in some.Reactions)
                {
                    if (reaction is TReactionEmoji emoji)
                    {
                        availableReactions.Add(emoji.Emoticon);
                    }
                    else if (reaction is TReactionCustomEmoji customEmoji)
                    {
                        allowCustomReaction = true;
                        availableReactions.Add($"custom_{customEmoji.DocumentId}");
                    }
                }
                break;
        }

        if (peer.PeerType == PeerType.Channel)
        {
            var command = new SetChannelAvailableReactionsCommand(
                ChannelId.Create(peer.PeerId),
                input.ToRequestInfo(),
                reactionType,
                allowCustomReaction,
                availableReactions,
                obj.ReactionsLimit);

            await _commandBus.PublishAsync(command, CancellationToken.None);
        }
        else if (peer.PeerType == PeerType.Chat)
        {
            var command = new SetChatAvailableReactionsCommand(
                ChatId.Create(peer.PeerId),
                input.ToRequestInfo(),
                reactionType,
                allowCustomReaction,
                availableReactions,
                obj.ReactionsLimit);

            await _commandBus.PublishAsync(command, CancellationToken.None);
        }
        else
        {
            RpcErrors.RpcErrors400.PeerIdInvalid.ThrowRpcError();
        }

        var date = DateTime.UtcNow.ToTimestamp();
        return new TUpdates
        {
            Updates = new TVector<IUpdate>(),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = date,
            Seq = 0
        };
    }
}
