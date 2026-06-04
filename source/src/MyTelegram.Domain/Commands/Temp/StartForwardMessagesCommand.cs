namespace MyTelegram.Domain.Commands.Temp;

public class StartForwardMessagesCommand(
    TempId aggregateId,
    RequestInfo requestInfo,
    bool silent,
    bool background,
    bool withMyScore,
    bool dropAuthor,
    bool dropMediaCaptions,
    bool noForwards,
    Peer fromPeer,
    Peer toPeer,
    List<int> messageIds,
    List<long> randomIds,
    int? scheduleDate,
    Peer? sendAs,
    bool forwardFromLinkedChannel,
    bool post,
    int? ttlPeriod,
    Dictionary<long, string>? fromNames)
    : RequestCommand2<TempAggregate, TempId, IExecutionResult>(aggregateId, requestInfo)
{
    public bool Silent { get; } = silent;
    public bool Background { get; } = background;
    public bool WithMyScore { get; } = withMyScore;
    public bool DropAuthor { get; } = dropAuthor;
    public bool DropMediaCaptions { get; } = dropMediaCaptions;
    public bool NoForwards { get; } = noForwards;
    public Peer FromPeer { get; } = fromPeer;
    public Peer ToPeer { get; } = toPeer;
    public List<int> MessageIds { get; } = messageIds;
    public List<long> RandomIds { get; } = randomIds;
    public int? ScheduleDate { get; } = scheduleDate;
    public Peer? SendAs { get; } = sendAs;
    public bool ForwardFromLinkedChannel { get; } = forwardFromLinkedChannel;
    public bool Post { get; } = post;

    public int? TtlPeriod { get; } = ttlPeriod;
    public Dictionary<long, string>? FromNames { get; } = fromNames;
}