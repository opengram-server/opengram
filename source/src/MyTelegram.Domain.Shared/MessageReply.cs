namespace MyTelegram;

public class MessageReply(long? channelId, int replies, int repliesPts, int? maxId, List<Peer>? recentRepliers)
{
    public long? ChannelId { get; set; } = channelId;
    public int Replies { get; set; } = replies;
    public int RepliesPts { get; set; } = repliesPts;

    public int? MaxId { get; set; } = maxId;
    public List<Peer>? RecentRepliers { get; set; } = recentRepliers;
}