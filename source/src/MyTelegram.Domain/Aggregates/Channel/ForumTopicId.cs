namespace MyTelegram.Domain.Aggregates.Channel;

public class ForumTopicId(string value) : Identity<ForumTopicId>(value)
{
    public static ForumTopicId Create(long channelId,
        int topicId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"{channelId}_{topicId}");
    }
}