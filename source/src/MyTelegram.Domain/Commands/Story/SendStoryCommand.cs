using MyTelegram.Domain.Aggregates.Story;

namespace MyTelegram.Domain.Commands.Story;

public class SendStoryCommand(
    StoryId aggregateId,
    RequestInfo requestInfo,
    long peerId,
    int storyId,
    byte[] media,
    string? caption,
    List<long>? privacyRules,
    int date,
    int expireDate,
    bool pinned,
    bool noForwards,
    bool isPublic) : Command<StoryAggregate, StoryId, IExecutionResult>(aggregateId)
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public long PeerId { get; } = peerId;
    public int StoryId { get; } = storyId;
    public byte[] Media { get; } = media;
    public string? Caption { get; } = caption;
    public List<long>? PrivacyRules { get; } = privacyRules;
    public int Date { get; } = date;
    public int ExpireDate { get; } = expireDate;
    public bool Pinned { get; } = pinned;
    public bool NoForwards { get; } = noForwards;
    public bool IsPublic { get; } = isPublic;
}
