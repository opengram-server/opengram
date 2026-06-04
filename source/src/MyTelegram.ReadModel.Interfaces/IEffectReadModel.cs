namespace MyTelegram.ReadModel.Interfaces;

public interface IEffectReadModel : IReadModel
{
    long EffectId { get; }
    string Emoticon { get; }
    bool PremiumRequired { get; }
    long? StaticIconId { get; }
    long? EffectStickerId { get; }
    long? EffectAnimationId { get; }
}