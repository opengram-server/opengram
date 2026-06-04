namespace MyTelegram.ReadModel.Impl;

public class MessageEffectReadModel : IMessageEffectReadModel
{
    public string Id { get; private set; } = null!;
    public long? Version { get; set; }
    
    public long EffectId { get; private set; }
    public string Emoticon { get; private set; } = null!;
    public long? StaticIconId { get; private set; }
    public long EffectStickerId { get; private set; }
    public long? EffectAnimationId { get; private set; }
    public bool PremiumRequired { get; private set; }

    public MessageEffectReadModel(long effectId, string emoticon, long? staticIconId, long effectStickerId, long? effectAnimationId, bool premiumRequired)
    {
        Id = effectId.ToString();
        EffectId = effectId;
        Emoticon = emoticon;
        StaticIconId = staticIconId;
        EffectStickerId = effectStickerId;
        EffectAnimationId = effectAnimationId;
        PremiumRequired = premiumRequired;
    }
    
    // Default constructor for serialization
    public MessageEffectReadModel() { }
}
