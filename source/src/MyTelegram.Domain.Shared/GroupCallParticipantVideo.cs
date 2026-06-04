#pragma warning disable
namespace MyTelegram;

public class GroupCallParticipantVideo
{
    public bool? Paused { get; set; }
    public string Endpoint { get; set; }
    public int? AudioSource { get; set; }
    public List<GroupCallParticipantVideoSourceGroup> SourceGroups { get; set; }
}