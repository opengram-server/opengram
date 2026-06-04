namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Channels;

///<summary>
/// Reorder pinned forum topics
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHANNEL_INVALID The provided channel is invalid.
/// See <a href="https://corefork.telegram.org/method/channels.reorderPinnedForumTopics" />
///</summary>
internal sealed class ReorderPinnedForumTopicsHandler : RpcResultObjectHandler<MyTelegram.Schema.Channels.RequestReorderPinnedForumTopics, MyTelegram.Schema.IUpdates>,
    Channels.IReorderPinnedForumTopicsHandler
{
    protected override Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Channels.RequestReorderPinnedForumTopics obj)
    {
        return Task.FromResult<IUpdates>(new TUpdates
        {
            Updates = [],
            Chats = [],
            Users = [],
            Date = CurrentDate
        });
    }
}
