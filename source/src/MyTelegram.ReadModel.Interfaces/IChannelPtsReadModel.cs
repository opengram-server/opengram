namespace MyTelegram.ReadModel.Interfaces;

public interface IChannelPtsReadModel : IReadModel
{
    long UserId { get; }
    //long ChannelId { get; }
    //long Pts { get; }
    long GlobalSeqNo { get; }
}