using MyTelegram.Queries.Stories;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// Obtain info about the view count of boths of stories by their IDs.
/// See <a href="https://corefork.telegram.org/method/stories.getStoriesViews" />
///</summary>
internal sealed class GetStoriesViewsHandler(
    IQueryProcessor queryProcessor)
    : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestGetStoriesViews, MyTelegram.Schema.Stories.IStoryViews>,
    Stories.IGetStoriesViewsHandler
{
    protected override async Task<MyTelegram.Schema.Stories.IStoryViews> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestGetStoriesViews obj)
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

        var viewsList = new List<Schema.IStoryViews>();

        foreach (var storyId in obj.Id)
        {
            var story = await queryProcessor.ProcessAsync(
                new MyTelegram.Queries.Stories.GetStoryByIdQuery(peerId, storyId),
                CancellationToken.None);

            viewsList.Add(new TStoryViews
            {
                ViewsCount = story is { IsDeleted: false } ? story.ViewsCount : 0,
                HasViewers = story is { IsDeleted: false }
            });
        }

        return new Schema.Stories.TStoryViews
        {
            Views = new TVector<Schema.IStoryViews>(viewsList),
            Users = []
        };
    }
}
