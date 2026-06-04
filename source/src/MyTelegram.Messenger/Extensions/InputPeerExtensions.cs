using MyTelegram.Schema;

namespace MyTelegram.Messenger.Extensions;

public static class InputPeerExtensions
{
    public static long GetPeerId(this IInputPeer inputPeer)
    {
        return inputPeer switch
        {
            TInputPeerUser peerUser => peerUser.UserId,
            TInputPeerChat peerChat => peerChat.ChatId,
            TInputPeerChannel peerChannel => peerChannel.ChannelId,
            TInputPeerSelf => 0, // Will be replaced with actual user ID by caller
            TInputPeerUserFromMessage userFromMessage => userFromMessage.UserId,
            TInputPeerChannelFromMessage channelFromMessage => channelFromMessage.ChannelId,
            _ => 0
        };
    }
}
