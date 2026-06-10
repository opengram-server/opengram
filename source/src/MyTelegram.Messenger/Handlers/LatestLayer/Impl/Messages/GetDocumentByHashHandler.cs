namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Get a document by its SHA256 hash, mainly used for gifs.
/// See <a href="https://corefork.telegram.org/method/messages.getDocumentByHash" />
///</summary>
internal sealed class GetDocumentByHashHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetDocumentByHash, MyTelegram.Schema.IDocument>,
    Messages.IGetDocumentByHashHandler
{
    protected override Task<IDocument> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetDocumentByHash obj)
    {
        // Validate the hash is a proper SHA256 (32 bytes)
        if (obj.Sha256.IsEmpty || obj.Sha256.Length != 32)
        {
            throw new RpcException(new RpcError(400, "SHA256_HASH_INVALID"));
        }

        // Document deduplication by hash is not implemented yet.
        // Per Telegram spec, returning TDocumentEmpty tells the client
        // "not cached, upload it yourself".
        return Task.FromResult<IDocument>(new TDocumentEmpty());
    }
}
