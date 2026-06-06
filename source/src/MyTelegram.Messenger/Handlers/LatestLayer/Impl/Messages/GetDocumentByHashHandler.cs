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
        // Document not found by hash - this is normal when the file
        // hasn't been uploaded yet. Return TDocumentEmpty.
        return Task.FromResult<IDocument>(new TDocumentEmpty());
    }
}
