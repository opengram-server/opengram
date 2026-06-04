using MyTelegram.Schema.Account;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Fetch the full list of only the IDs of songs currently added to the profile
/// See <a href="https://corefork.telegram.org/method/account.getSavedMusicIds" />
///</summary>
internal sealed class GetSavedMusicIdsHandler(IQueryProcessor queryProcessor)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestGetSavedMusicIds, MyTelegram.Schema.Account.ISavedMusicIds>
{
    protected override async Task<MyTelegram.Schema.Account.ISavedMusicIds> HandleCoreAsync(
        IRequestInput input,
        RequestGetSavedMusicIds obj)
    {
        var savedMusic = await queryProcessor.ProcessAsync(
            new GetSavedMusicByUserIdQuery(input.UserId),
            CancellationToken.None);

        if (savedMusic == null || savedMusic.DocumentIds.Count == 0)
        {
            return new MyTelegram.Schema.Account.TSavedMusicIds
            {
                Ids = new TVector<long>()
            };
        }

        // Calculate hash (XOR of all IDs)
        var hash = 0L;
        foreach (var id in savedMusic.DocumentIds)
        {
            hash ^= id;
        }

        // Check if hash matches
        if (obj.Hash == hash)
        {
            return new MyTelegram.Schema.Account.TSavedMusicIdsNotModified();
        }

        return new MyTelegram.Schema.Account.TSavedMusicIds
        {
            Ids = new TVector<long>(savedMusic.DocumentIds)
        };
    }
}
