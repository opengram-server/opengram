namespace MyTelegram.Messenger.Services;

public class SetPrivacyOutput(
    TVector<IPrivacyRule> rules,
    TVector<IChat> chats,
    TVector<IUser> users)
{
    public TVector<IPrivacyRule> Rules { get; } = rules;
    public TVector<IChat> Chats { get; } = chats;
    public TVector<IUser> Users { get; } = users;
}