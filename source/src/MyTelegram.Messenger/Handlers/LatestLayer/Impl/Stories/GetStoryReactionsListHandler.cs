// ReSharper disable All

using MyTelegram.Schema.Stories;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// See <a href="https://corefork.telegram.org/method/stories.getStoryReactionsList" />
///</summary>
internal sealed class GetStoryReactionsListHandler : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestGetStoryReactionsList, MyTelegram.Schema.Stories.IStoryReactionsList>,
    Stories.IGetStoryReactionsListHandler
{
    protected override Task<MyTelegram.Schema.Stories.IStoryReactionsList> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestGetStoryReactionsList obj)
    {
        return Task.FromResult<IStoryReactionsList>(new TStoryReactionsList
        {
            Chats = [],
            Users = [],
            Reactions = []
        });
    }
}
