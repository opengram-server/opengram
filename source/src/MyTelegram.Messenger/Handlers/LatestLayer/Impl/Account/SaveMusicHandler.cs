using MyTelegram.Domain.Aggregates.SavedMusic;
using MyTelegram.Domain.Commands.SavedMusic;
using MyTelegram.Schema.Account;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Adds or removes a song from the current user's profile
/// See <a href="https://corefork.telegram.org/method/account.saveMusic" />
///</summary>
internal sealed class SaveMusicHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestSaveMusic, IBool>
{
    protected override async Task<IBool> HandleCoreAsync(
        IRequestInput input,
        RequestSaveMusic obj)
    {
        // Extract document ID
        if (obj.Id is not TInputDocument inputDocument)
        {
            RpcErrors.RpcErrors400.FileIdInvalid.ThrowRpcError();
            return new TBoolFalse(); // unreachable but needed for compiler
        }

        var documentId = inputDocument.Id;
        
        // Extract after_id if specified
        long? afterDocumentId = null;
        if (obj.AfterId is TInputDocument afterInputDocument)
        {
            afterDocumentId = afterInputDocument.Id;
            
            // Verify after_id exists in user's saved music
            var savedMusic = await queryProcessor.ProcessAsync(
                new GetSavedMusicByUserIdQuery(input.UserId),
                CancellationToken.None);
                
            if (savedMusic == null || !savedMusic.DocumentIds.Contains(afterDocumentId.Value))
            {
                // Return false if after_id not found
                return new TBoolFalse();
            }
        }

        // Create aggregate ID
        var aggregateId = new SavedMusicId($"savedmusic-{input.UserId}");

        // Create command
        var command = new SaveMusicCommand(
            aggregateId,
            input.ToRequestInfo(),
            input.UserId,
            documentId,
            obj.Unsave,
            afterDocumentId);

        await commandBus.PublishAsync(command, CancellationToken.None);

        return new TBoolTrue();
    }
}
