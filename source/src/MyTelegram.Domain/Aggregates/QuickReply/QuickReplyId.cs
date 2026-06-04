using MyTelegram.Domain.Aggregates.QuickReply;

namespace MyTelegram.Domain.Aggregates.QuickReply;

public class QuickReplyId : Identity<QuickReplyId>
{
    public QuickReplyId(string value) : base(value)
    {
    }

    public static QuickReplyId Create(long userId)
    {
        return new QuickReplyId($"quickreply_{userId}");
    }
}
