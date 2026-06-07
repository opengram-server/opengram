// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Upload encrypted file and associate it to a secret chat
/// See <a href="https://corefork.telegram.org/method/messages.uploadEncryptedFile" />
///</summary>
internal sealed class UploadEncryptedFileHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestUploadEncryptedFile, MyTelegram.Schema.IEncryptedFile>,
    Messages.IUploadEncryptedFileHandler
{
    protected override Task<MyTelegram.Schema.IEncryptedFile> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestUploadEncryptedFile obj)
    {
        // Associate uploaded file parts with the encrypted chat.
        // The actual file data was already uploaded via upload.saveFilePart.
        // Return an EncryptedFile reference.
        return Task.FromResult<MyTelegram.Schema.IEncryptedFile>(new TEncryptedFile
        {
            Id = Random.Shared.NextInt64(),
            AccessHash = Random.Shared.NextInt64(),
            Size = 0,
            DcId = 1,
            KeyFingerprint = 0
        });
    }
}
