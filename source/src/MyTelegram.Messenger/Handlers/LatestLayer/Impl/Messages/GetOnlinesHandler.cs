namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Get count of online users in a chat.
/// See <a href="https://corefork.telegram.org/method/messages.getOnlines" />
///</summary>
internal sealed class GetOnlinesHandler(
    IQueryProcessor queryProcessor,
    IPeerHelper peerHelper,
    IAccessHashHelper accessHashHelper,
    IUserAppService userAppService,
    ILogger<GetOnlinesHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetOnlines, MyTelegram.Schema.IChatOnlines>,
    Messages.IGetOnlinesHandler
{
    protected override async Task<IChatOnlines> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetOnlines obj)
    {
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);
        await accessHashHelper.CheckAccessHashAsync(input, obj.Peer);

        int onlineCount = 1; // At least the requesting user

        try
        {
            IReadOnlyCollection<long> memberIds;

            if (peerHelper.IsChannelPeer(peer.PeerId))
            {
                memberIds = await queryProcessor.ProcessAsync(
                    new GetChannelMemberListQuery(peer.PeerId),
                    CancellationToken.None);
            }
            else
            {
                memberIds = await queryProcessor.ProcessAsync(
                    new GetChatMemberListQuery(peer.PeerId),
                    CancellationToken.None);
            }

            if (memberIds.Count > 0)
            {
                // Load user read models to check IsOnline
                var userReadModels = await userAppService.GetListAsync(memberIds.ToList());
                onlineCount = userReadModels.Count(u => u.IsOnline);

                // At minimum, the requesting user is online
                if (onlineCount == 0)
                {
                    onlineCount = 1;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "GetOnlines: Error checking online status for peer {PeerId}", peer.PeerId);
        }

        return new TChatOnlines
        {
            Onlines = onlineCount
        };
    }
}
