// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// See <a href="https://corefork.telegram.org/method/messages.getAvailableEffects" />
///</summary>
internal sealed class GetAvailableEffectsHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetAvailableEffects, MyTelegram.Schema.Messages.IAvailableEffects>,
    Messages.IGetAvailableEffectsHandler
{
    private readonly IQueryProcessor _queryProcessor;

    public GetAvailableEffectsHandler(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    protected override async Task<MyTelegram.Schema.Messages.IAvailableEffects> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetAvailableEffects obj)
    {
        var effects = await _queryProcessor.ProcessAsync(new GetAvailableEffectsQuery());
        var tAvailableEffects = new TAvailableEffects
        {
            Documents = [], // TODO: Fetch documents if needed
            Effects = new TVector<IAvailableEffect>(effects.Select(e => new TAvailableEffect
            {
                Id = e.EffectId,
                Emoticon = e.Emoticon,
                StaticIconId = e.StaticIconId,
                EffectStickerId = e.EffectStickerId,
                EffectAnimationId = e.EffectAnimationId,
                PremiumRequired = e.PremiumRequired
            }).ToList())
        };

        return tAvailableEffects;
    }
}
