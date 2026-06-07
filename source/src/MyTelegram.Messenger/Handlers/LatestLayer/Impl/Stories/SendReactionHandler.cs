using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.Commands.Story;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// React to a story.
/// See <a href="https://corefork.telegram.org/method/stories.sendReaction" />
///</summary>
internal sealed class SendReactionHandler(
    ICommandBus commandBus)
    : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestSendReaction, MyTelegram.Schema.IUpdates>,
    Stories.ISendReactionHandler
{
    protected override async Task<IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestSendReaction obj)
    {
        // Resolve peer
        long peerId = input.UserId;
        if (obj.Peer is TInputPeerUser inputPeerUser)
        {
            peerId = inputPeerUser.UserId;
        }
        else if (obj.Peer is TInputPeerChannel inputPeerChannel)
        {
            peerId = inputPeerChannel.ChannelId;
        }
        else if (obj.Peer is TInputPeerSelf)
        {
            peerId = input.UserId;
        }

        // Extract reaction string
        string reaction = obj.Reaction switch
        {
            TReactionEmoji reactionEmoji => reactionEmoji.Emoticon,
            TReactionCustomEmoji customEmoji => $"custom_{customEmoji.DocumentId}",
            _ => ""
        };

        if (!string.IsNullOrEmpty(reaction))
        {
            var command = new AddStoryReactionCommand(
                StoryId.Create(peerId, obj.StoryId),
                input.ToRequestInfo(),
                input.UserId,
                reaction);

            await commandBus.PublishAsync(command, CancellationToken.None);
        }

        return null!;
    }
}
