namespace MyTelegram.Domain.Sagas.Snapshots;

public class ClearHistorySagaSnapshot(
    long reqMsgId,
    long selfAuthKeyId,
    long selfPermAuthKeyId,
    long selfUserId,
    PeerType toPeerType,
    long toPeerId,
    bool revoke,
    string messageActionData,
    long randomId,
    bool needWaitForClearParticipantHistory,
    int nextMaxId,
    int totalCountToBeDelete,
    Dictionary<long, int> peerToPts,
    Dictionary<long, List<int>> ownerToMessageIdList)
    : ISnapshot
{
    public string MessageActionData { get; } = messageActionData;

    public bool NeedWaitForClearParticipantHistory { get; } = needWaitForClearParticipantHistory;

    //public int ParticipantHistoryCount { get; }
    //public int DeletedParticipantHistoryCount { get; }
    public int NextMaxId { get; } = nextMaxId;

    public Dictionary<long, List<int>> OwnerToMessageIdList { get; } = ownerToMessageIdList;

    //public int DeletedCount { get; private set; }
    public Dictionary<long, int> PeerToPts { get; } = peerToPts;
    public long RandomId { get; } = randomId;

    public long ReqMsgId { get; } = reqMsgId;
    public bool Revoke { get; } = revoke;
    public long SelfAuthKeyId { get; } = selfAuthKeyId;
    public long SelfPermAuthKeyId { get; } = selfPermAuthKeyId;
    public long SelfUserId { get; } = selfUserId;
    public long ToPeerId { get; } = toPeerId;
    public PeerType ToPeerType { get; } = toPeerType;
    public int TotalCountToBeDelete { get; } = totalCountToBeDelete;
}
