namespace MyTelegram.Messenger.Services;

public record SearchPostsResult(
    IReadOnlyCollection<IMessageReadModel> Messages,
    IReadOnlyCollection<IChannelReadModel> Channels,
    IReadOnlyCollection<IChannelMemberReadModel> ChannelMembers,
    IReadOnlyCollection<IPhotoReadModel> Photos,
    IReadOnlyCollection<IUserReadModel> Users);