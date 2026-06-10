using MyTelegram.Domain.Aggregates.Story;

namespace MyTelegram.Domain.Commands.Story;

public class EditStoryCommand(
    StoryId aggregateId,
    RequestInfo requestInfo,
    byte[]? media,
    string? caption,
    List<long>? privacyRules)
    : RequestCommand2<StoryAggregate, StoryId, IExecutionResult>(aggregateId, requestInfo)
{
    public byte[]? Media { get; } = media;
    public string? Caption { get; } = caption;
    public List<long>? PrivacyRules { get; } = privacyRules;
}
