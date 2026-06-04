namespace MyTelegram.Domain.Commands.Messaging;

public class EditOutboxMessageCommand(
    MessageId aggregateId,
    RequestInfo requestInfo,
    int messageId,
    string newMessage,
    TVector<IMessageEntity>? entities,
    int editDate,
    IMessageMedia? media,
    IReplyMarkup? replyMarkup,
    List<long>? chatMembers,
    bool invertMedia,
    List<string>? hashtags)
    : RequestCommand2<MessageAggregate, MessageId, IExecutionResult>(aggregateId, requestInfo)
{
    public int MessageId { get; } = messageId;
    public string NewMessage { get; } = newMessage;
    public TVector<IMessageEntity>? Entities { get; } = entities;
    public int EditDate { get; } = editDate;
    public IMessageMedia? Media { get; } = media;
    public IReplyMarkup? ReplyMarkup { get; } = replyMarkup;
    public List<long>? ChatMembers { get; } = chatMembers;
    public bool InvertMedia { get; } = invertMedia;
    public List<string>? Hashtags { get; } = hashtags;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return RequestInfo.RequestId.ToByteArray();
    }
}