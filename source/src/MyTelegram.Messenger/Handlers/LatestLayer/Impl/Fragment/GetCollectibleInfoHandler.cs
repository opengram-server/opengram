// ReSharper disable All

using MyTelegram.Schema.Fragment;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Fragment;

///<summary>
/// Fetch information about a fragment collectible (username or phone number from Fragment auction)
/// <para>Possible errors</para>
/// Code Type Description
/// 400 COLLECTIBLE_INVALID The specified collectible is invalid.
/// 400 COLLECTIBLE_NOT_FOUND The specified collectible could not be found.
/// See <a href="https://corefork.telegram.org/method/fragment.getCollectibleInfo" />
///</summary>
internal sealed class GetCollectibleInfoHandler : RpcResultObjectHandler<MyTelegram.Schema.Fragment.RequestGetCollectibleInfo, MyTelegram.Schema.Fragment.ICollectibleInfo>,
    Fragment.IGetCollectibleInfoHandler
{
    protected override async Task<MyTelegram.Schema.Fragment.ICollectibleInfo> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Fragment.RequestGetCollectibleInfo obj)
    {
        // Get collectible info from Fragment auction system
        // obj.Collectible can be TInputCollectibleUsername or TInputCollectiblePhone
        
        string itemType = "";
        string itemValue = "";
        
        if (obj.Collectible is TInputCollectibleUsername username)
        {
            itemType = "username";
            itemValue = username.Username;
        }
        else if (obj.Collectible is TInputCollectiblePhone phone)
        {
            itemType = "number";
            itemValue = phone.Phone;
        }
        
        // TODO: Query fragment_auctions collection from MongoDB to get real purchase data
        // For now, return data that matches Fragment style with TON currency
        
        return new TCollectibleInfo
        {
            PurchaseDate = CurrentDate - (86400 * 30), // 30 days ago
            Currency = "TON",
            Amount = 183750, // TON amount (Fragment uses TON)
            CryptoCurrency = "TON",
            CryptoAmount = 183750,
            Url = $"https://fragment.com/{itemType}/{itemValue}"
        };
    }
}
