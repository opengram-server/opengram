namespace MyTelegram.Domain.Events.Messaging;

//public class MessageReactionUpdatedEvent : AggregateEvent<MessageAggregate, MessageId>
//{
//    public long MessageOwnerPeerId { get; }
//    public long ReactionSenderUserId { get; }
//    public List<string>? EmojiReactions { get; }
//    public List<long>? CustomEmojiRections { get; }

//    public MessageReactionUpdatedEvent(long messageOwnerPeerId, long reactionSenderUserId, List<string>? emojiReactions, List<long>? customEmojiRections)
//    {
//        MessageOwnerPeerId = messageOwnerPeerId;
//        ReactionSenderUserId = reactionSenderUserId;
//        EmojiReactions = emojiReactions;
//        CustomEmojiRections = customEmojiRections;
//    }
//}

public class CheckMessageViewLogSuccessEvent(
    RequestInfo requestInfo,
    int messageId,
    bool alreadyIncremented)
    : RequestAggregateEvent2<MessageViewLogAggregate, MessageViewLogId>(requestInfo)
{
    public bool AlreadyIncremented { get; } = alreadyIncremented;

    public int MessageId { get; } = messageId;
}