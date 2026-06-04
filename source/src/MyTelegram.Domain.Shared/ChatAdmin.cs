namespace MyTelegram;

public class ChatAdmin(
    long promotedBy,
    bool canEdit,
    long userId,
    ChatAdminRights adminRights,
    string rank)
{
    public ChatAdminRights AdminRights { get; internal set; } = adminRights;

    /// <summary>
    ///     Can this admin promote other admins with the same permissions?
    /// </summary>
    public bool CanEdit { get; init; } = canEdit;

    /// <summary>
    ///     User that promoted the user to admin
    /// </summary>
    public long PromotedBy { get; init; } = promotedBy;

    public string Rank { get; init; } = rank;
    public long UserId { get; init; } = userId;

    public void SetAdminRights(ChatAdminRights adminRights)
    {
        AdminRights = adminRights;
    }
}