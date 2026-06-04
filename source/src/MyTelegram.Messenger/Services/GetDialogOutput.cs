namespace MyTelegram.Messenger.Services;

public class GetDialogOutput(
    long selfUserId,
    IReadOnlyCollection<IDialogReadModel> dialogList,
    IReadOnlyCollection<IMessageReadModel> messageList,
    IReadOnlyCollection<IUserReadModel> userList,
    IReadOnlyCollection<IPhotoReadModel> photoList,
    IReadOnlyCollection<IChatReadModel> chatList,
    IReadOnlyCollection<IChannelReadModel> channelList,
    IReadOnlyCollection<IContactReadModel> contactList,
    IReadOnlyCollection<IPrivacyReadModel> privacyList,
    IReadOnlyCollection<IChannelMemberReadModel> channelMemberList,
    IReadOnlyCollection<IPollReadModel>? pollList,
    IReadOnlyCollection<IPollAnswerVoterReadModel>? chosenPollOptions,
    IReadOnlyCollection<IUserReactionReadModel>? userReactionList,
    int limit)
{
    //,
    //PtsReadModel ptsReadModel
    //IReadOnlyCollection<PeerNotifySettingsReadModel> peerNotifySettingList
    //PtsReadModel = ptsReadModel;
    //PeerNotifySettingList = peerNotifySettingList;

    public int CachedPts { get; set; }
    public IReadOnlyCollection<IChannelReadModel> ChannelList { get; set; } = channelList;
    public IReadOnlyCollection<IChannelMemberReadModel> ChannelMemberList { get; set; } = channelMemberList;
    public IReadOnlyCollection<IPollReadModel>? PollList { get; } = pollList;
    public IReadOnlyCollection<IPollAnswerVoterReadModel>? ChosenPollOptions { get; } = chosenPollOptions;
    public IReadOnlyCollection<IUserReactionReadModel>? UserReactionList { get; } = userReactionList;
    public int Limit { get; } = limit;
    public IReadOnlyCollection<IChatReadModel> ChatList { get; set; } = chatList;

    public IReadOnlyCollection<IContactReadModel> ContactList { get; } = contactList;

    public IReadOnlyCollection<IDialogReadModel> DialogList { get; set; } = dialogList;
    public IReadOnlyCollection<IMessageReadModel> MessageList { get; set; } = messageList;
    public IReadOnlyCollection<IPrivacyReadModel> PrivacyList { get; } = privacyList;
    public IPtsReadModel? PtsReadModel { get; set; }

    public long SelfUserId { get; set; } = selfUserId;

    public IReadOnlyCollection<IUserReadModel> UserList { get; set; } = userList;

    public IReadOnlyCollection<IPhotoReadModel> PhotoList { get; set; } = photoList;
    //public IReadOnlyCollection<PeerNotifySettingsReadModel> PeerNotifySettingList { get; }
}