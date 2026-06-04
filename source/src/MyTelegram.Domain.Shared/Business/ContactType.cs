namespace MyTelegram.Domain.Shared.Business;

/// <summary>
/// Represents the contact relationship between two users
/// </summary>
public enum ContactType
{
    /// <summary>
    /// No contact relationship
    /// </summary>
    None,
    
    /// <summary>
    /// Self - the user is viewing their own profile
    /// </summary>
    Self,
    
    /// <summary>
    /// Target user is in my contacts
    /// </summary>
    TargetUserIsMyContact,
    
    /// <summary>
    /// I am in the target user's contacts (but target is not in mine)
    /// </summary>
    ContactOfTargetUser,
    
    /// <summary>
    /// Mutual contact - both users have each other in contacts
    /// </summary>
    Mutual,
    
    /// <summary>
    /// Target user is not in my contacts
    /// </summary>
    TargetUserIsNotMyContact
}
