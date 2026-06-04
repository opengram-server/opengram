#pragma warning disable
namespace MyTelegram;

public class GroupCallParticipant
{
    public string Endpoint { get; set; }
    public bool? Muted { get; set; }
    public bool Left { get; set; }
    public bool CanSelfUnmute { get; set; }
    public bool JustJoined { get; set; }
    public bool Versioned { get; set; }
    public bool Min { get; set; }
    //public bool MutedByYou { get; set; }
    public long? MutedByUserId { get; set; }
    public bool VolumeByAdmin { get; set; }
    public bool VideoJoined { get; set; }
    public long UserId { get; set; }
    public int Date { get; set; }
    public int? ActiveDate { get; set; }

    public int Source { get; set; }
    public int? Volume { get; set; }
    public string? About { get; set; }
    public long? RaiseHandRating { get; set; }
    public bool? VideoStopped { get; set; }
    public bool? VideoPaused { get; set; }
    public bool? PresentationPaused { get; set; }

    public GroupCallParticipantVideo? Video { get; set; }
    public GroupCallParticipantVideo? Presentation { get; set; }
}