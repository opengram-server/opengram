namespace MyTelegram.Messenger.Services.Interfaces;

public interface IMediaHelper
{
    MessageType GeMessageType(IMessageMedia? media);

    Task<IEncryptedFile> SaveEncryptedFileAsync(long reqMsgId,
        IInputEncryptedFile encryptedFile);

    Task<IMessageMedia?> SaveMediaAsync(IInputMedia? media, long userId = 0);

    Task<SavePhotoResult> SavePhotoAsync(long reqMsgId,
        long userId,
        long fileId,
        bool hasVideo,
        double? videoStartTs,
        int parts,
        string? name,
        string? md5,
        IVideoSize? videoEmojiMarkup = null,
        bool isProfilePhoto = false
    );
}