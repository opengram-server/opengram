using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.Commands.Story;
using MyTelegram.Queries.Stories;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// Mark all stories up to a certain ID as read, for a given peer.
/// See <a href="https://corefork.telegram.org/method/stories.readStories" />
///</summary>
internal sealed class ReadStoriesHandler(
    IQueryProcessor queryProcessor,
    ICommandBus commandBus)
    : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestReadStories, TVector<int>>,
    Stories.IReadStoriesHandler
{
    protected override async Task<TVector<int>> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestReadStories obj)
    {
        // Resolve peer ID
        long peerId = input.UserId;
        if (obj.Peer is TInputPeerUser inputPeerUser)
        {
            peerId = inputPeerUser.UserId;
        }
        else if (obj.Peer is TInputPeerChannel inputPeerChannel)
        {
            peerId = inputPeerChannel.ChannelId;
        }

        // Get all stories for the peer up to MaxId and increment views
        var stories = await queryProcessor.ProcessAsync(
            new GetPeerStoriesQuery(input.UserId, peerId),
            CancellationToken.None);

        var readIds = new List<int>();

        if (stories != null)
        {
            foreach (var story in stories.Where(s => s.StoryId <= obj.MaxId && !s.IsDeleted))
            {
                // Increment view count for each read story
                var viewCommand = new IncrementStoryViewsCommand(
                    StoryId.Create(peerId, story.StoryId),
                    story.ViewsCount + 1);

                await commandBus.PublishAsync(viewCommand, CancellationToken.None);
                readIds.Add(story.StoryId);
            }
        }

        return new TVector<int>(readIds);
    }
}
