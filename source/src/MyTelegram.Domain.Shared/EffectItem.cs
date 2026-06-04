namespace MyTelegram;

public record EffectItem(long Id, string Emoticon, bool PremiumRequired, long? StaticIconId, long? EffectStickerId, long? EffectAnimationId);