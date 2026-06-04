namespace MyTelegram.Domain.Aggregates.Story;

public class StoryState : AggregateState<StoryAggregate, StoryId, StoryState>
{
    public long PeerId { get; set; }
    public int StoryId { get; set; }
    public byte[] Media { get; set; } = Array.Empty<byte>();
    public string? Caption { get; set; }
    public List<long>? PrivacyRules { get; set; }
    public int Date { get; set; }
    public int ExpireDate { get; set; }
    public bool Pinned { get; set; }
    public bool NoForwards { get; set; }
    public bool IsPublic { get; set; }
    public int ViewsCount { get; set; }
    public bool IsDeleted { get; set; }
}
