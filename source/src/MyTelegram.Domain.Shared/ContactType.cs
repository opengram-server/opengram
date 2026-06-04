namespace MyTelegram;

public enum ContactType
{
    None,
    /// <summary>
    /// Self - viewing own profile
    /// </summary>
    Self,
    TargetUserIsMyContact,
    /// <summary>
    /// I am the contact person of the target user
    /// </summary>
    ContactOfTargetUser,
    /// <summary>
    /// Target user is not in my contacts
    /// </summary>
    TargetUserIsNotMyContact,
    Mutual
}