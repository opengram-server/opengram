namespace MyTelegram.ReadModel.Interfaces;

public interface IBotReadModel : IReadModel
{
    string? About { get; }
    bool AllowAccessGroupMessages { get; }
    bool AllowJoinGroups { get; }
    string BotName { get; }
    List<BotCommand> Commands { get; }

    string? Description { get; }

    string Id { get; }

    long OwnerUserId { get; }

    string Token { get; }

    long UserId { get; }

    //bool AllowPrivacy { get; }
    string UserName { get; }
    string? WebHookUrl { get; }
    long? DescriptionDocumentId { get; }
    long? DescriptionPhotoId { get; }
    string? InlinePlaceholder { get; }
    bool InlineModeEnabled { get; }
    bool BusinessModeEnabled { get; }
    string? PrivacyPolicyUrl { get; }
    string? MiniAppUrl { get; }
    int ChatAdminRights { get; }
    int ChannelAdminRights { get; }
    long? ProfilePhotoId { get; }
}