namespace MyTelegram.Messenger.Services.Impl;

public static class PrivacyConverter
{
    public static PrivacyType ToPrivacyType(IInputPrivacyKey key)
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
            TInputPrivacyKeyStarGiftsAutoSave => PrivacyType.StarGiftsAutoSave,
            TInputPrivacyKeyNoPaidMessages => PrivacyType.NoPaidMessages,
            _ => throw new ArgumentException($"Unknown privacy key type: {key.GetType().Name}")
        };
    }

    public static IPrivacyKey ToPrivacyKey(PrivacyType privacyType)
    {
        return privacyType switch
        {
            PrivacyType.StatusTimestamp => new TPrivacyKeyStatusTimestamp(),
            PrivacyType.ChatInvite => new TPrivacyKeyChatInvite(),
            PrivacyType.PhoneCall => new TPrivacyKeyPhoneCall(),
            PrivacyType.PhoneP2P => new TPrivacyKeyPhoneP2P(),
            PrivacyType.Forwards => new TPrivacyKeyForwards(),
            PrivacyType.ProfilePhoto => new TPrivacyKeyProfilePhoto(),
            PrivacyType.PhoneNumber => new TPrivacyKeyPhoneNumber(),
            PrivacyType.AddedByPhone => new TPrivacyKeyAddedByPhone(),
            PrivacyType.VoiceMessages => new TPrivacyKeyVoiceMessages(),
            PrivacyType.About => new TPrivacyKeyAbout(),
            PrivacyType.Birthday => new TPrivacyKeyBirthday(),
            PrivacyType.StarGiftsAutoSave => new TPrivacyKeyStarGiftsAutoSave(),
            PrivacyType.NoPaidMessages => new TPrivacyKeyNoPaidMessages(),
            _ => throw new ArgumentException($"Unknown privacy type: {privacyType}")
        };
    }

    public static IInputPrivacyKey ToInputPrivacyKey(PrivacyType privacyType)
    {
        return privacyType switch
        {
            PrivacyType.StatusTimestamp => new TInputPrivacyKeyStatusTimestamp(),
            PrivacyType.ChatInvite => new TInputPrivacyKeyChatInvite(),
            PrivacyType.PhoneCall => new TInputPrivacyKeyPhoneCall(),
            PrivacyType.PhoneP2P => new TInputPrivacyKeyPhoneP2P(),
            PrivacyType.Forwards => new TInputPrivacyKeyForwards(),
            PrivacyType.ProfilePhoto => new TInputPrivacyKeyProfilePhoto(),
            PrivacyType.PhoneNumber => new TInputPrivacyKeyPhoneNumber(),
            PrivacyType.AddedByPhone => new TInputPrivacyKeyAddedByPhone(),
            PrivacyType.VoiceMessages => new TInputPrivacyKeyVoiceMessages(),
            PrivacyType.About => new TInputPrivacyKeyAbout(),
            PrivacyType.Birthday => new TInputPrivacyKeyBirthday(),
            PrivacyType.StarGiftsAutoSave => new TInputPrivacyKeyStarGiftsAutoSave(),
            PrivacyType.NoPaidMessages => new TInputPrivacyKeyNoPaidMessages(),
            _ => throw new ArgumentException($"Unknown privacy type: {privacyType}")
        };
    }

    public static PrivacyValueData ToPrivacyValueData(IInputPrivacyRule rule)
    {
        return rule switch
        {
            TInputPrivacyValueAllowContacts => new PrivacyValueData(PrivacyValueType.AllowContacts),
            TInputPrivacyValueAllowAll => new PrivacyValueData(PrivacyValueType.AllowAll),
            TInputPrivacyValueAllowUsers r => new PrivacyValueData(PrivacyValueType.AllowUsers, 
                System.Text.Json.JsonSerializer.Serialize(r.Users.Select(u => u switch
                {
                    TInputUser iu => iu.UserId,
                    _ => 0L
                }).ToList())),
            TInputPrivacyValueDisallowContacts => new PrivacyValueData(PrivacyValueType.DisallowContacts),
            TInputPrivacyValueDisallowAll => new PrivacyValueData(PrivacyValueType.DisallowAll),
            TInputPrivacyValueDisallowUsers r => new PrivacyValueData(PrivacyValueType.DisallowUsers,
                System.Text.Json.JsonSerializer.Serialize(r.Users.Select(u => u switch
                {
                    TInputUser iu => iu.UserId,
                    _ => 0L
                }).ToList())),
            TInputPrivacyValueAllowChatParticipants r => new PrivacyValueData(PrivacyValueType.AllowChatParticipants,
                System.Text.Json.JsonSerializer.Serialize(r.Chats.ToList())),
            TInputPrivacyValueDisallowChatParticipants r => new PrivacyValueData(PrivacyValueType.DisallowChatParticipants,
                System.Text.Json.JsonSerializer.Serialize(r.Chats.ToList())),
            TInputPrivacyValueAllowCloseFriends => new PrivacyValueData(PrivacyValueType.AllowCloseFriends),
            TInputPrivacyValueAllowPremium => new PrivacyValueData(PrivacyValueType.AllowPremium),
            TInputPrivacyValueAllowBots => new PrivacyValueData(PrivacyValueType.AllowBots),
            TInputPrivacyValueDisallowBots => new PrivacyValueData(PrivacyValueType.DisallowBots),
            _ => throw new ArgumentException($"Unknown privacy rule type: {rule.GetType().Name}")
        };
    }

    public static IPrivacyRule ToPrivacyRule(PrivacyValueData data)
    {
        return data.PrivacyValueType switch
        {
            PrivacyValueType.AllowContacts => new TPrivacyValueAllowContacts(),
            PrivacyValueType.AllowAll => new TPrivacyValueAllowAll(),
            PrivacyValueType.AllowUsers => new TPrivacyValueAllowUsers
            {
                Users = new TVector<long>(
                    System.Text.Json.JsonSerializer.Deserialize<List<long>>(data.JsonData ?? "[]") ?? new List<long>())
            },
            PrivacyValueType.DisallowContacts => new TPrivacyValueDisallowContacts(),
            PrivacyValueType.DisallowAll => new TPrivacyValueDisallowAll(),
            PrivacyValueType.DisallowUsers => new TPrivacyValueDisallowUsers
            {
                Users = new TVector<long>(
                    System.Text.Json.JsonSerializer.Deserialize<List<long>>(data.JsonData ?? "[]") ?? new List<long>())
            },
            PrivacyValueType.AllowChatParticipants => new TPrivacyValueAllowChatParticipants
            {
                Chats = new TVector<long>(
                    System.Text.Json.JsonSerializer.Deserialize<List<long>>(data.JsonData ?? "[]") ?? new List<long>())
            },
            PrivacyValueType.DisallowChatParticipants => new TPrivacyValueDisallowChatParticipants
            {
                Chats = new TVector<long>(
                    System.Text.Json.JsonSerializer.Deserialize<List<long>>(data.JsonData ?? "[]") ?? new List<long>())
            },
            PrivacyValueType.AllowCloseFriends => new TPrivacyValueAllowCloseFriends(),
            PrivacyValueType.AllowPremium => new TPrivacyValueAllowPremium(),
            PrivacyValueType.AllowBots => new TPrivacyValueAllowBots(),
            PrivacyValueType.DisallowBots => new TPrivacyValueDisallowBots(),
            _ => throw new ArgumentException($"Unknown privacy value type: {data.PrivacyValueType}")
        };
    }
}
