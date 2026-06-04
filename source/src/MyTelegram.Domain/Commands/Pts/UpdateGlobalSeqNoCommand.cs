namespace MyTelegram.Domain.Commands.Pts;

public class UpdateGlobalSeqNoCommand(
    PtsId aggregateId,
    long peerId,
    long permAuthKeyId,
    long globalSeqNo)
    : Command<PtsAggregate, PtsId, IExecutionResult>(aggregateId)
{
    public long GlobalSeqNo { get; } = globalSeqNo;
    public long PeerId { get; } = peerId;
    public long PermAuthKeyId { get; } = permAuthKeyId;
}

//public class UpdateChannelPtsForUserCommand : Command<ChannelPtsAggregate, ChannelPtsId, IExecutionResult>
//{
//    public long UserId { get; }
//    public long ChannelId { get; }
//    public int Pts { get; }
//    public long GlobalSeqNo { get; }

//    public UpdateChannelPtsForUserCommand(ChannelPtsId aggregateId,long userId,long channelId,int pts,long globalSeqNo) : base(aggregateId)
//    {
//        UserId = userId;
//        ChannelId = channelId;
//        Pts = pts;
//        GlobalSeqNo = globalSeqNo;
//    }
//}