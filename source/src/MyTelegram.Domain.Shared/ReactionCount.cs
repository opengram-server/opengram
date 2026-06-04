using MyTelegram.Schema.Extensions;

namespace MyTelegram;

public class MessageReactor
{
    public bool Top { get; set; }
    public bool My { get; set; }
    public bool Anonymous { get; set; }
    public Peer? PeerId { get; set; }
    public int Count { get; set; }
}

public class MessagePeerReaction
{
    public bool Big { get; set; }
    public required Peer PeerId { get; set; }
    public long SenderUserId { get; set; }
    public int Date { get; set; }
    public required IReaction Reaction { get; set; }
}

public class ReactionCount(
    IReaction reaction,
    int count,
    string? emoticon,
    long? customEmojiDocumentId
    )
{
    public IReaction Reaction { get; init; } = reaction;

    public int Count { get; set; } = count;
    public string? Emoticon { get; internal set; } = emoticon;
    public long? CustomEmojiDocumentId { get; internal set; } = customEmojiDocumentId;
    //public int Count { get; set; } = count;

    //public int? ChosenOrder { get; set; }

    //public void IncrementCount()
    //{
    //    Count++;
    //}

    //public void DecrementCount()
    //{
    //    Count--;
    //}

    public IReaction GetReaction()
    {
        if (Reaction != null!)
        {
            return Reaction;
        }

        // For compatibility with old data
        if (CustomEmojiDocumentId.HasValue)
        {
            return new TReactionCustomEmoji { DocumentId = CustomEmojiDocumentId.Value };
        }

        if (!string.IsNullOrEmpty(Emoticon))
        {
            return new TReactionEmoji
            {
                Emoticon = Emoticon
            };
        }

        return new TReactionEmpty();
    }

    public long GetReactionId()
    {
        return GetReaction().GetReactionId();
    }
}