namespace MyTelegram.Messenger.Services;

public class SearchContactOutput(
    long selfUserId,
    IReadOnlyCollection<IUserReadModel> userList,
    IReadOnlyCollection<IPhotoReadModel> photoList,
    IReadOnlyCollection<IContactReadModel> contactList,
    IReadOnlyCollection<IChannelReadModel> myChannelList,
    IReadOnlyCollection<IChannelReadModel> channelList,
    IReadOnlyCollection<IPrivacyReadModel> privacyList,
    IReadOnlyCollection<IChannelMemberReadModel> channelMemberList)
{
    public IReadOnlyCollection<IChannelReadModel> ChannelList { get; } = channelList;
    public IReadOnlyCollection<IChannelMemberReadModel> ChannelMemberList { get; } = channelMemberList;
    public IReadOnlyCollection<IContactReadModel> ContactList { get; } = contactList;
    public IReadOnlyCollection<IChannelReadModel> MyChannelList { get; } = myChannelList;
    public IReadOnlyCollection<IPrivacyReadModel> PrivacyList { get; } = privacyList;

    public long SelfUserId { get; } = selfUserId;
    public IReadOnlyCollection<IUserReadModel> UserList { get; } = userList;
    public IReadOnlyCollection<IPhotoReadModel> PhotoList { get; } = photoList;
}