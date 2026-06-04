namespace MyTelegram.Messenger.Services.Caching;

public interface IAccessHashHelper
{
    Task<bool> IsAccessHashValidAsync(IRequestWithAccessHashKeyId request, long id,
        long accessHash, AccessHashType? accessHashType = null);

    Task CheckAccessHashAsync(IRequestWithAccessHashKeyId request, long id,
        long accessHash, AccessHashType? accessHashType = null);

    Task CheckAccessHashAsync(IRequestWithAccessHashKeyId request, IInputPeer? inputPeer);
    Task CheckAccessHashAsync(IRequestWithAccessHashKeyId request, IInputUser inputUser);
    Task CheckAccessHashAsync(IRequestWithAccessHashKeyId request, IInputChannel inputChannel);
    //Task CheckAccessHashAsync(Peer peer);
    void AddAccessHash(long id, long accessHash);
}
