using MyTelegram.Queries.Stories;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// Generate a story deep link for a specific story.
/// See <a href="https://corefork.telegram.org/method/stories.exportStoryLink" />
///</summary>
internal sealed class ExportStoryLinkHandler(
    IQueryProcessor queryProcessor,
    IUserAppService userAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestExportStoryLink, MyTelegram.Schema.IExportedStoryLink>,
    Stories.IExportStoryLinkHandler
{
    protected override async Task<IExportedStoryLink> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestExportStoryLink obj)
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

        // Get user info for username-based link
        var user = await userAppService.GetAsync(peerId);
        var username = user?.UserName ?? peerId.ToString();

        // Generate deep link per corefork.telegram.org spec
        var link = $"https://t.me/{username}/s/{obj.Id}";

        return new TExportedStoryLink
        {
            Link = link
        };
    }
}
