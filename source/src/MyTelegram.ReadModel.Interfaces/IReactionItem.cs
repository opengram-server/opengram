namespace MyTelegram.ReadModel.Interfaces;

public interface IReactionItem
{
    List<ReactionCount>? Reactions { get; set; }
    List<MessagePeerReaction>? RecentReactions2 { get; set; }
    List<MessageReactor>? TopReactors { get; }
    int MessageId { get; }
    IInputReplyTo? ReplyTo { get; }
}
