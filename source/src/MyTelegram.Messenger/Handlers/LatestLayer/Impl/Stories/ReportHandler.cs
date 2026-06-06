namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// Report a story.
/// See <a href="https://corefork.telegram.org/method/stories.report" />
///</summary>
internal sealed class ReportHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestReport, MyTelegram.Schema.IReportResult>,
    Stories.IReportHandler
{
    protected override Task<IReportResult> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestReport obj)
    {
        // Accept the report — in a self-hosted environment, reporting is a no-op
        // but we acknowledge receipt per the API contract
        return Task.FromResult<IReportResult>(new TReportResultReported());
    }
}
