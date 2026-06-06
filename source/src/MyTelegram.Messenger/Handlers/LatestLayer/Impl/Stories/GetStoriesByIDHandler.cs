using MyTelegram.Queries.Stories;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// Obtain full info about a set of stories by their IDs.
/// See <a href="https://corefork.telegram.org/method/stories.getStoriesByID" />
///</summary>
internal sealed class GetStoriesByIDHandler(
    IQueryProcessor queryProcessor,
    IUserConverterService userConverterService)
    : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestGetStoriesByID, MyTelegram.Schema.Stories.IStories>,
    Stories.IGetStoriesByIDHandler
{
    protected override async Task<MyTelegram.Schema.Stories.IStories> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestGetStoriesByID obj)
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
        else if (obj.Peer is TInputPeerSelf)
        {
            peerId = input.UserId;
        }

        var storyItems = new List<IStoryItem>();
        var userIds = new HashSet<long>();

        foreach (var storyId in obj.Id)
        {
            var story = await queryProcessor.ProcessAsync(
                new GetStoryByIdQuery(peerId, storyId),
                CancellationToken.None);

            if (story != null && !story.IsDeleted)
            {
                storyItems.Add(new TStoryItem
                {
                    Id = story.StoryId,
                    Date = story.Date,
                    ExpireDate = story.ExpireDate,
                    Caption = story.Caption,
                    Pinned = story.Pinned,
                    Noforwards = story.NoForwards,
                    Media = story.Media,
                    Views = new TStoryViews
                    {
                        ViewsCount = story.ViewsCount
                    },
                    Entities = story.Entities != null ? new TVector<IMessageEntity>(story.Entities) : null
                });
                userIds.Add(story.OwnerPeerId);
            }
            else
            {
                // Return TStoryItemDeleted for deleted/not found stories
                storyItems.Add(new TStoryItemDeleted { Id = storyId });
            }
        }

        var users = await userConverterService.GetUserListAsync(input, userIds.ToList(), false, false, input.Layer);

        return new MyTelegram.Schema.Stories.TStories
        {
            Count = storyItems.Count,
            Stories = new TVector<IStoryItem>(storyItems),
            Users = new TVector<IUser>(users),
            Chats = []
        };
    }
}
