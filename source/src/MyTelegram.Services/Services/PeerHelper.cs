using System.Diagnostics.CodeAnalysis;

namespace MyTelegram.Services.Services;

public class PeerHelper : IPeerHelper, ITransientDependency
{
    public Peer GetChannel(IInputChannel channel)
    {
        return channel.ToChannelPeer();
    }

    [return: NotNullIfNotNull(nameof(peer))]
    public Peer? GetPeer(IInputPeer? peer,
        long selfUserId = 0)
    {
        return peer?.ToPeer(selfUserId);
    }

    public Peer GetPeer(IInputUser userPeer,
        long selfUserId)
    {
        return userPeer.ToPeer(selfUserId);
    }

    public bool IsChannelPeer(long peerId)
    {
        return peerId >= MyTelegramConsts.ChannelInitId;
    }

    public bool IsUserPeer(long peerId)
    {
        return peerId is >= MyTelegramConsts.UserIdInitId and < MyTelegramConsts.BotUserInitId;
    }

    public IPeer ToPeer(Peer peer)
    {
        return ToPeer(peer.PeerType, peer.PeerId);
    }

    public IPeer ToPeer(PeerType peerType,
        long peerId)
    {
        return peerType switch
        {
            PeerType.Self => new TPeerUser { UserId = peerId },
            PeerType.User => new TPeerUser { UserId = peerId },
            PeerType.Empty => new TPeerUser { UserId = peerId },
            PeerType.Chat => new TPeerChat { ChatId = peerId },
            PeerType.Channel => new TPeerChannel { ChannelId = peerId },
            _ => throw new ArgumentOutOfRangeException(nameof(peerType),
                $"Unable to convert Peer(PeerType= {peerType}, PeerId= {peerId}) to IPeer")
        };
    }

    public bool IsBotUser(long userId)
    {
        return userId is >= MyTelegramConsts.BotUserInitId and <= MyTelegramConsts.ChatIdInitId;
    }

    public PeerType GetPeerType(long peerId)
    {
        var peerType = peerId switch
        {
            < MyTelegramConsts.ChatIdInitId => PeerType.User,
            >= MyTelegramConsts.ChatIdInitId and < MyTelegramConsts.ChannelInitId => PeerType.Chat,
            >= MyTelegramConsts.ChannelInitId => PeerType.Channel
        };

        return peerType;
    }

    public Peer GetPeer(long peerId)
    {
        var peerType = GetPeerType(peerId);
        return new Peer(peerType, peerId);
    }

    public bool IsEncryptedDialogPeer(long dialogId)
    {
        //-9223372036854775808=ulong 0x8000000000000000L
        return (dialogId & 0x4000000000000000L) != 0 && (dialogId & -9223372036854775808) == 0;
    }
}