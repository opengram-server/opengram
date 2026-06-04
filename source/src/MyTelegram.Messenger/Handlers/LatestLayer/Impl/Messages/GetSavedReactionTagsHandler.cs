namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Fetch the full list of <a href="https://corefork.telegram.org/api/saved-messages#tags">saved message tags</a> created by the user.
/// See <a href="https://corefork.telegram.org/method/messages.getSavedReactionTags" />
///</summary>
internal sealed class GetSavedReactionTagsHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetSavedReactionTags, MyTelegram.Schema.Messages.ISavedReactionTags>,
    Messages.IGetSavedReactionTagsHandler
{
    protected override Task<MyTelegram.Schema.Messages.ISavedReactionTags> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetSavedReactionTags obj)
    {
        return Task.FromResult<MyTelegram.Schema.Messages.ISavedReactionTags>(new TSavedReactionTags
        {
            Tags = []
        });
    }
}
