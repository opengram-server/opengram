using EventFlow.ReadStores;

namespace MyTelegram.ReadModel;

public interface IMessageEffectReadModel : IReadModel
{
    long EffectId { get; }
    string Emoticon { get; }
    long? StaticIconId { get; }
    long EffectStickerId { get; }
    long? EffectAnimationId { get; }
    bool PremiumRequired { get; }
}
