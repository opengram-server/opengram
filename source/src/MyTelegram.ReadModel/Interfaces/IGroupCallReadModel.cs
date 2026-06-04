namespace MyTelegram.ReadModel.Interfaces;

public interface IGroupCallReadModel : IReadModel
{
    long CallId { get; }
    long AccessHash { get; }
    long PeerId { get; }
    string PeerType { get; }
    string? Title { get; }
    int ParticipantsCount { get; }
    bool JoinMuted { get; }
    bool CanChangeJoinMuted { get; }
    bool CanStartVideo { get; }
    bool RecordVideoActive { get; }
    bool RtmpStream { get; }
    int? StreamDcId { get; }
    int? RecordStartDate { get; }
    int? ScheduleDate { get; }
    int UnmutedVideoCount { get; }
    int UnmutedVideoLimit { get; }
    long? Version { get; }
    int CreatedDate { get; }
    int? EndedDate { get; }
    int? Duration { get; }
    bool IsDiscarded { get; }
    List<GroupCallParticipantReadModel> Participants { get; }
}

public class GroupCallParticipantReadModel
{
    public long PeerId { get; set; }
    public string PeerType { get; set; } = default!;
    public int Source { get; set; }
    public int JoinedDate { get; set; }
    public int? ActiveDate { get; set; }
    public int? LeftDate { get; set; }
    public bool Muted { get; set; }
    public bool CanSelfUnmute { get; set; }
    public int Volume { get; set; }
    public string? About { get; set; }
    public long? RaiseHandRating { get; set; }
    public bool? VideoStopped { get; set; }
    public bool Left { get; set; }
    public string? Params { get; set; }
}
