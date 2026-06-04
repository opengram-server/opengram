using MyTelegram.Schema;

namespace MyTelegram.Messenger.Services.Interfaces;

public interface IStarGiftAttributeGenerator
{
    TVector<IStarGiftAttribute> GenerateAttributes(long giftId);
    TVector<IStarGiftAttribute> GenerateUpgradeAttributes(long giftId, string uniqueId);
    TVector<IStarGiftAttribute> GenerateSampleAttributesForPreview(long giftId);
}
