using MyTelegram.Schema;
using MyTelegram.Schema.Payments;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/method/payments.getStarGiftUpgradePreview" />
///</summary>
internal sealed class GetStarGiftUpgradePreviewHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestGetStarGiftUpgradePreview, MyTelegram.Schema.Payments.IStarGiftUpgradePreview>,
    Payments.IGetStarGiftUpgradePreviewHandler
{
    private readonly IStarGiftAttributeGenerator _attributeGenerator;

    public GetStarGiftUpgradePreviewHandler(IStarGiftAttributeGenerator attributeGenerator)
    {
        _attributeGenerator = attributeGenerator;
    }

    protected override Task<MyTelegram.Schema.Payments.IStarGiftUpgradePreview> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestGetStarGiftUpgradePreview obj)
    {
        // Generate multiple sample attributes (1 model, 1 pattern, 5 backdrops) for preview animation
        var attributes = _attributeGenerator.GenerateSampleAttributesForPreview(obj.GiftId);
        
        return Task.FromResult<MyTelegram.Schema.Payments.IStarGiftUpgradePreview>(new TStarGiftUpgradePreview
        {
            SampleAttributes = attributes
        });
    }
}
