using MyTelegram.Domain.Shared;
using MyTelegram.Schema;

namespace MyTelegram.Messenger.Services.Interfaces;

/// <summary>
/// Service for managing Action Bar settings according to Telegram API specification
/// https://corefork.telegram.org/api/action-bar
/// </summary>
public interface IActionBarAppService
{
    /// <summary>
    /// Creates peer settings for new chats based on contact relationship
    /// </summary>
    PeerSettings CreateInitialPeerSettings(long userId, long peerId, MyTelegram.ContactType contactType, MyTelegram.Domain.Shared.Business.PeerType peerType);

    /// <summary>
    /// Updates peer settings when contact relationship changes
    /// </summary>
    PeerSettings UpdatePeerSettingsOnContactChange(PeerSettings currentSettings, MyTelegram.ContactType oldContactType, MyTelegram.ContactType newContactType);

    /// <summary>
    /// Determines which action bar type should be shown based on current settings
    /// </summary>
    ActionBarType DetermineActionBarType(PeerSettings settings);
}

public enum ActionBarType
{
    None,
    ReportSpamBlockAddContact,
    ReportSpamOrUnarchive,
    AddContact,
    ShareContact,
    ReportIrrelevantGeolocation,
    InviteNewMembers,
    JoinRequestAdmin,
    ManageBusinessBot,
    BotAds,
    AccountInformation
}
