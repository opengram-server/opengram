using Microsoft.Extensions.Logging;
using MyTelegram.Queries.Stories;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// Get all active stories from contacts and subscriptions
/// See <a href="https://corefork.telegram.org/method/stories.getAllStories" />
///</summary>
internal sealed class GetAllStoriesHandler(
    ILogger<GetAllStoriesHandler> logger,
    IQueryProcessor queryProcessor) 
    : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestGetAllStories, MyTelegram.Schema.Stories.IAllStories>,
    Stories.IGetAllStoriesHandler
{
    protected override async Task<MyTelegram.Schema.Stories.IAllStories> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestGetAllStories obj)
    {
        // Get all stories from MongoDB
        var stories = await queryProcessor.ProcessAsync(
            new GetAllActiveStoriesQuery(input.UserId, obj.State),
            CancellationToken.None);

        if (stories == null || stories.Count == 0)
        {
            return new MyTelegram.Schema.Stories.TAllStoriesNotModified
            {
                State = obj.State ?? "",
                StealthMode = new TStoriesStealthMode
                {
                    ActiveUntilDate = null,
                    CooldownUntilDate = null
                }
            };
        }

        // Group stories by peer
        var peerStories = stories
            .GroupBy(s => s.OwnerPeerId)
            .Select(g => new TPeerStories
            {
                Peer = new TPeerUser { UserId = g.Key },
                MaxReadId = g.Max(s => s.StoryId),
                Stories = new TVector<IStoryItem>(
                    g.Select(s => new TStoryItem
                    {
                        Id = s.StoryId,
                        Date = s.Date,
                        ExpireDate = s.ExpireDate,
                        Caption = s.Caption,
                        Pinned = s.Pinned,
                        Noforwards = s.NoForwards,
                        Views = new TStoryViews
                        {
                            ViewsCount = s.ViewsCount
                        }
                    } as IStoryItem).ToList())
            })
            .ToList();

        return new MyTelegram.Schema.Stories.TAllStories
        {
            HasMore = false,
            Count = peerStories.Count,
            State = DateTime.UtcNow.Ticks.ToString(),
            PeerStories = new TVector<IPeerStories>(peerStories.Cast<IPeerStories>().ToList()),
            Chats = new TVector<IChat>(),
            Users = new TVector<IUser>(),
            StealthMode = new TStoriesStealthMode
            {
                ActiveUntilDate = null,
                CooldownUntilDate = null
            }
        };
    }
}
