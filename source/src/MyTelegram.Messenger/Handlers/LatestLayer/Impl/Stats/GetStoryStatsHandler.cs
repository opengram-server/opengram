namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stats;

///<summary>
/// Get <a href="https://corefork.telegram.org/api/stats">statistics</a> for a certain <a href="https://corefork.telegram.org/api/stories">story</a>.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// See <a href="https://corefork.telegram.org/method/stats.getStoryStats" />
///</summary>
internal sealed class GetStoryStatsHandler : RpcResultObjectHandler<MyTelegram.Schema.Stats.RequestGetStoryStats, MyTelegram.Schema.Stats.IStoryStats>,
    Stats.IGetStoryStatsHandler
{
    protected override Task<MyTelegram.Schema.Stats.IStoryStats> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stats.RequestGetStoryStats obj)
    {
        throw new NotImplementedException();
    }
}
