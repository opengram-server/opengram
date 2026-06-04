namespace MyTelegram.ReadModel.Interfaces;

public interface IForumTopicReadModel : IReadModel
{
    long ChannelId { get; }
    int TopicId { get; }
    //ForumTopic ForumTopic { get; }
    string Title { get; }
    long? IconEmojiId { get; }
    int? IconColor { get; }
    Peer? SendAs { get; }
    bool Pinned { get; }
    int Date { get; }
    int TopMessage { get; }
    bool My { get; }
    bool Closed { get; }
    int ReadInboxMaxId { get; }
    int ReadOutboxMaxId { get; }
    int UnreadCount { get; }
    int UnreadMentionsCount { get; }
    int UnreadReactionsCount { get; }
}