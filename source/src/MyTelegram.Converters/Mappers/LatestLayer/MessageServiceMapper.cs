using System.Diagnostics.CodeAnalysis;

namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class MessageServiceMapper
    : IObjectMapper<IMessageReadModel, TMessageService>,
        IObjectMapper<MessageItem, TMessageService>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public TMessageService Map(IMessageReadModel source)
    {
        return Map(source, new TMessageService());
    }

    public TMessageService Map(
        IMessageReadModel source,
        TMessageService destination
    )
    {
        destination.Out = source.Out;
        //destination.Mentioned = source.Mentioned;
        //destination.MediaUnread = source.MediaUnread;
        //destination.ReactionsArePossible = source.ReactionsArePossible;
        destination.Silent = source.Silent;
        destination.Post = source.Post;
        //destination.Legacy = source.Legacy;
        destination.Id = source.MessageId;
        //destination.FromId = source.FromId;

        var peer = new Peer(source.ToPeerType, source.ToPeerId);
        destination.PeerId = peer.ToPeer();

        destination.ReplyTo = source.ReplyTo.ToMessageReplyHeader();
        destination.ReplyTo = source.ReplyTo.ToMessageReplyHeader();
        destination.Date = source.Date;
        destination.Action = source.MessageAction ?? source.MessageActionData?.ToBytes().ToTObject<IMessageAction>();
        //destination.Reactions = source.Reactions;
        destination.TtlPeriod = source.TtlPeriod;

        //if (destination.Action is TMessageActionChatAddUser)
        //{
        //    destination.ReactionsArePossible = true;
        //}
        switch (destination.Action)
        {
            case TMessageActionChatAddUser:
            case TMessageActionChatEditPhoto:
            case TMessageActionChatEditTitle:
            case TMessageActionChatJoinedByLink:
            case TMessageActionChatJoinedByRequest:
            case TMessageActionSetChatWallPaper:
            case TMessageActionSetChatTheme:
                destination.ReactionsArePossible = true;
                break;
        }

        return destination;
    }

    [return: NotNullIfNotNull("source")]
    public TMessageService? Map(MessageItem source)
    {
        return Map(source, new TMessageService());
    }

    [return: NotNullIfNotNull("source")]
    public TMessageService? Map(MessageItem source, TMessageService destination)
    {
        destination.Out = source.IsOut;
        //destination.Mentioned = source.Mentioned;
        //destination.MediaUnread = source.MediaUnread;
        //destination.ReactionsArePossible = source.ReactionsArePossible;
        destination.Silent = source.Silent;
        destination.Post = source.Post; // source.Post;
        //destination.Legacy = source.Legacy;
        destination.Id = source.MessageId;
        //destination.FromId = source.FromId;

        destination.PeerId = source.ToPeer.ToPeer();

        destination.ReplyTo = source.InputReplyTo.ToMessageReplyHeader();
        destination.Date = source.Date;
        destination.Action = source.MessageAction;
        //destination.Reactions = source.Reactions;
        destination.TtlPeriod = source.TtlPeriod;

        switch (destination.Action)
        {
            case TMessageActionChatAddUser:
            case TMessageActionChatEditPhoto:
            case TMessageActionChatEditTitle:
            case TMessageActionChatJoinedByLink:
            case TMessageActionChatJoinedByRequest:
            case TMessageActionSetChatWallPaper:
            case TMessageActionSetChatTheme:
                destination.ReactionsArePossible = true;
                break;
        }

        return destination;
    }
}