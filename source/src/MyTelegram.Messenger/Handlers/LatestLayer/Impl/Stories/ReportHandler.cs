namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// Report a story.
/// See <a href="https://corefork.telegram.org/method/stories.report" />
///</summary>
internal sealed class ReportHandler(
    ILogger<ReportHandler> logger,
    IPeerHelper peerHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestReport, MyTelegram.Schema.IReportResult>,
    Stories.IReportHandler
{
    protected override Task<IReportResult> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestReport obj)
    {
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);

        // Log the report for administrative review on self-hosted
        logger.LogInformation(
            "StoryReport: UserId={UserId} reported peer={PeerId} stories=[{StoryIds}] message={Message}",
            input.UserId,
            peer.PeerId,
            string.Join(",", obj.Id),
            obj.Message ?? "");

        // On self-hosted, reporting is accepted and logged.
        // Server admins can review logs for moderation.
        return Task.FromResult<IReportResult>(new TReportResultReported());
    }
}
