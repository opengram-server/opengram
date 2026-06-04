namespace MyTelegram.ReadModel.Interfaces;

public interface IGroupCallParticipantReadModel : IReadModel
{
    public long ChannelId { get; }
    public long GroupCallId { get; }
    public long UserId { get; }
    GroupCallParticipant Participant { get; }
}