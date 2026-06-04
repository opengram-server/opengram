using MyTelegram.Schema;

namespace MyTelegram;

public static class PrivacyTypeExtensions
{
    public static PrivacyType ToPrivacyType(this IInputPrivacyKey key)
    {
        return key switch
        {
            TInputPrivacyKeyStatusTimestamp => PrivacyType.StatusTimestamp,
            TInputPrivacyKeyChatInvite => PrivacyType.ChatInvite,
            TInputPrivacyKeyPhoneCall => PrivacyType.PhoneCall,
            TInputPrivacyKeyPhoneP2P => PrivacyType.PhoneP2P,
            TInputPrivacyKeyForwards => PrivacyType.Forwards,
            TInputPrivacyKeyProfilePhoto => PrivacyType.ProfilePhoto,
            TInputPrivacyKeyPhoneNumber => PrivacyType.PhoneNumber,
            TInputPrivacyKeyAddedByPhone => PrivacyType.AddedByPhone,
            TInputPrivacyKeyVoiceMessages => PrivacyType.VoiceMessages,
            TInputPrivacyKeyAbout => PrivacyType.About,
            TInputPrivacyKeyBirthday => PrivacyType.Birthday,
            _ => PrivacyType.Unknown
        };
    }
}
