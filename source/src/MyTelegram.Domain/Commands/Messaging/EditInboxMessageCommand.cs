namespace MyTelegram.Domain.Commands.Messaging;

public class EditInboxMessageCommand(
    MessageId aggregateId,
    RequestInfo requestInfo,
    int messageId,
    string newMessage,
    int editDate,
    TVector<IMessageEntity>? entities,
    IMessageMedia? media,
    IReplyMarkup? replyMarkup,
    bool invertMedia,
    List<string>? hashtags
    )
    : RequestCommand2<MessageAggregate, MessageId, IExecutionResult>(aggregateId, requestInfo)
{
    public int MessageId { get; } = messageId;
    public string NewMessage { get; } = newMessage;
    public int EditDate { get; } = editDate;
    public TVector<IMessageEntity>? Entities { get; } = entities;
    public IMessageMedia? Media { get; } = media;
    public IReplyMarkup? ReplyMarkup { get; } = replyMarkup;
    public bool InvertMedia { get; } = invertMedia;
    public List<string>? Hashtags { get; } = hashtags;
}