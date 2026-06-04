namespace MyTelegram.Domain.Sagas.States;

public class ForwardMessageState : AggregateState<ForwardMessageSaga, ForwardMessageSagaId, ForwardMessageState>,
    IApply<ForwardMessageSagaStartedSagaEvent>,
    IApply<ForwardSingleMessageSuccessSagaEvent>,
    IApply<MessageReplyCreatedSagaEvent>
{
    public int ForwardCount { get; private set; }
    public Peer FromPeer { get; private set; } = null!;
    public IReadOnlyList<int> IdList { get; private set; } = null!;

    public bool IsCompleted => ForwardCount == IdList.Count;
    public IReadOnlyList<long> RandomIdList { get; private set; } = null!;
    public RequestInfo RequestInfo { get; private set; } = default!;
    public Peer ToPeer { get; private set; } = null!;
    public bool ForwardFromLinkedChannel { get; private set; }
    public bool Post { get; private set; }
    public int? TtlPeriod { get; private set; }
    public Dictionary<long, string>? FromNames { get; private set; }
    public Peer? SendAs { get; private set; }
    public void Apply(ForwardMessageSagaStartedSagaEvent aggregateEvent)
    {
        RequestInfo = aggregateEvent.RequestInfo;
        FromPeer = aggregateEvent.FromPeer;
        ToPeer = aggregateEvent.ToPeer;
        IdList = aggregateEvent.IdList;
        RandomIdList = aggregateEvent.RandomIdList;
        ForwardFromLinkedChannel = aggregateEvent.ForwardFromLinkedChannel;
        Post = aggregateEvent.Post;
        TtlPeriod = aggregateEvent.TtlPeriod;
        FromNames = aggregateEvent.FromNames;
        SendAs = aggregateEvent.SendAs;
    }

    public void Apply(ForwardSingleMessageSuccessSagaEvent aggregateEvent)
    {
        ForwardCount++;
    }

    public void Apply(MessageReplyCreatedSagaEvent aggregateEvent)
    {
    }
}
