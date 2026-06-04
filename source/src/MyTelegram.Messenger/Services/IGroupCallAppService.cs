namespace MyTelegram.Messenger.Services;

public interface IGroupCallAppService
{
    Task<bool> IsAdminAsync(long userId, long peerId, PeerType peerType);
    Task<bool> PeerExistsAsync(long peerId, PeerType peerType);
    Task BroadcastUpdateToParticipantsAsync(long callId, IUpdate update, List<long>? excludeUserIds = null);
    Task BroadcastGroupCallUpdateAsync(long peerId, PeerType peerType, IGroupCall groupCall);
}
