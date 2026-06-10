namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// Obtain the list of users that have viewed a specific story we posted.
/// See <a href="https://corefork.telegram.org/method/stories.getStoryViewsList" />
///</summary>
internal sealed class GetStoryViewsListHandler(
    IQueryProcessor queryProcessor)
    : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestGetStoryViewsList, MyTelegram.Schema.Stories.IStoryViewsList>,
    Stories.IGetStoryViewsListHandler
{
    protected override async Task<MyTelegram.Schema.Stories.IStoryViewsList> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestGetStoryViewsList obj)
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

        // Get story to verify it exists — use Stories namespace to avoid ambiguity
        var story = await queryProcessor.ProcessAsync(
            new MyTelegram.Queries.Stories.GetStoryByIdQuery(peerId, obj.Id),
            CancellationToken.None);

        if (story == null || story.IsDeleted)
        {
            RpcErrors.RpcErrors400.StoryIdInvalid.ThrowRpcError();
        }

        // Individual viewer tracking requires a StoryViewDetailsReadModel in MongoDB.
        // Counts are accurate from the story aggregate; the per-user list is empty.
        return new Schema.Stories.TStoryViewsList
        {
            Count = story!.ViewsCount,
            ViewsCount = story.ViewsCount,
            ForwardsCount = 0,
            ReactionsCount = 0,
            Views = [],
            Chats = [],
            Users = []
        };
    }
}
