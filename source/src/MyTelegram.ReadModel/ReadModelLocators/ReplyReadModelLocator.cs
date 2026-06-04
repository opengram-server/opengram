using System.Text.Json.Serialization;
using EventFlow.Core;
using EventFlow.ValueObjects;

namespace MyTelegram.ReadModel.ReadModelLocators;

[JsonConverter(typeof(SingleValueObject<ReplyId>))]
public class ReplyId(string value) : Identity<ReplyId>(value)
{
    public static ReplyId Create(long channelId, long messageId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"reply-{channelId}-{messageId}");
    }
}

public class ReplyReadModelLocator : IReplyReadModelLocator, ITransientDependency
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        var aggregateEvent = domainEvent.GetAggregateEvent();

        switch (aggregateEvent)
        {
            case MessageReplyUpdatedEvent messageReplyUpdatedEvent:
                yield return ReplyId.Create(messageReplyUpdatedEvent.OwnerChannelId, messageReplyUpdatedEvent.MessageId)
                    .Value;
                break;

            case ReplyBroadcastChannelCompletedSagaEvent replyBroadcastChannelCompletedSagaEvent:
                yield return ReplyId.Create(replyBroadcastChannelCompletedSagaEvent.ChannelId,
                    replyBroadcastChannelCompletedSagaEvent.MessageId).Value;
                break;

            case ReplyChannelMessageCompletedEvent replyChannelMessageCompletedEvent:
                yield return ReplyId.Create(replyChannelMessageCompletedEvent.ChannelId, replyChannelMessageCompletedEvent.ReplyToMessageId).Value;
                break;
            case MessageReplyCreatedSagaEvent messageReplyCreatedSagaEvent:

                yield return ReplyId
                    .Create(messageReplyCreatedSagaEvent.ChannelId, messageReplyCreatedSagaEvent.MessageId).Value;
                break;

        }
    }
}
