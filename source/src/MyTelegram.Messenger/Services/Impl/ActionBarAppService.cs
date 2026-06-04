using Microsoft.Extensions.Logging;
using MyTelegram.Schema;
using MyTelegram.Domain.Shared;
using MyTelegram.Messenger.Services.Interfaces;

namespace MyTelegram.Messenger.Services.Impl;

public class ActionBarAppService(
    ILogger<ActionBarAppService> logger,
    IPeerHelper peerHelper) : IActionBarAppService, ITransientDependency
{
    public MyTelegram.PeerSettings CreateInitialPeerSettings(long userId, long peerId, MyTelegram.ContactType contactType, MyTelegram.Domain.Shared.Business.PeerType peerType)
    {
        logger.LogDebug("Creating initial peer settings for user {UserId} -> peer {PeerId}, contactType: {ContactType}, peerType: {PeerType}",
            userId, peerId, contactType, peerType);

        var settings = new MyTelegram.PeerSettings();

        if (peerType == MyTelegram.Domain.Shared.Business.PeerType.User)
        {
            // For users, set initial action bar based on contact relationship
            switch (contactType)
            {
                case MyTelegram.ContactType.None:
                    // Non-contact user - show report spam/add contact/block
                    settings.ReportSpam = true;
                    settings.AddContact = true;
                    settings.BlockContact = true;
                    break;
                case MyTelegram.ContactType.TargetUserIsMyContact:
                    // User is my contact - show share contact
                    settings.ShareContact = true;
                    break;
                case MyTelegram.ContactType.Mutual:
                    // Mutual contact - hide action bar
                    break;
                case MyTelegram.ContactType.ContactOfTargetUser:
                    // Target user has me as contact - show add contact
                    settings.AddContact = true;
                    break;
            }
        }
        else if (peerType == MyTelegram.Domain.Shared.Business.PeerType.Channel)
        {
            // For channels, initially no action bar
            // Invite members flag may be set for recently created groups
        }

        return settings;
    }

    public MyTelegram.PeerSettings UpdatePeerSettingsOnContactChange(MyTelegram.PeerSettings currentSettings, MyTelegram.ContactType oldContactType, MyTelegram.ContactType newContactType)
    {
        logger.LogDebug("Updating peer settings on contact change from {OldContactType} to {NewContactType}", 
            oldContactType, newContactType);

        var wasContact = oldContactType == MyTelegram.ContactType.Mutual || oldContactType == MyTelegram.ContactType.TargetUserIsMyContact;
        var isContact = newContactType == MyTelegram.ContactType.Mutual || newContactType == MyTelegram.ContactType.TargetUserIsMyContact;

        var updatedSettings = new MyTelegram.PeerSettings
        {
            // Copy existing settings
            AddContact = currentSettings.AddContact,
            BlockContact = currentSettings.BlockContact,
            NeedContactsException = currentSettings.NeedContactsException,
            ReportGeo = currentSettings.ReportGeo,
            ReportSpam = currentSettings.ReportSpam,
            ShareContact = currentSettings.ShareContact,
            Autoarchived = currentSettings.Autoarchived,
            InviteMembers = currentSettings.InviteMembers,
            RequestChatBroadcast = currentSettings.RequestChatBroadcast,
        };

        if (!wasContact && isContact)
        {
            // User became a contact - hide action bar
            updatedSettings.ReportSpam = false;
            updatedSettings.AddContact = false;
            updatedSettings.BlockContact = false;
            
            // Set share contact if they added us
            if (newContactType == MyTelegram.ContactType.Mutual)
            {
                updatedSettings.ShareContact = true;
            }
        }
        else if (wasContact && !isContact)
        {
            // User removed from contacts - show action bar again
            updatedSettings.ReportSpam = true;
            updatedSettings.AddContact = true;
            updatedSettings.BlockContact = true;
            updatedSettings.ShareContact = false;
        }

        return updatedSettings;
    }

    public MyTelegram.Messenger.Services.Interfaces.ActionBarType DetermineActionBarType(MyTelegram.PeerSettings settings)
    {
        // Check for mutually exclusive action bar types according to specification
        
        // 1. Report spam, block or add contact
        if (settings.ReportSpam && settings.AddContact && settings.BlockContact)
        {
            return MyTelegram.Messenger.Services.Interfaces.ActionBarType.ReportSpamBlockAddContact;
        }

        // 2. Report spam or unarchive
        if (settings.ReportSpam && !settings.AddContact && !settings.BlockContact)
        {
            return MyTelegram.Messenger.Services.Interfaces.ActionBarType.ReportSpamOrUnarchive;
        }

        // 3. Add contact
        if (settings.AddContact && !settings.ReportSpam && !settings.BlockContact)
        {
            return MyTelegram.Messenger.Services.Interfaces.ActionBarType.AddContact;
        }

        // 4. Share contact
        if (settings.ShareContact && !settings.AddContact && !settings.ReportSpam && !settings.BlockContact)
        {
            return MyTelegram.Messenger.Services.Interfaces.ActionBarType.ShareContact;
        }

        // 5. Report irrelevant geolocation
        if (settings.ReportGeo && !settings.AddContact && !settings.ReportSpam && !settings.BlockContact)
        {
            return MyTelegram.Messenger.Services.Interfaces.ActionBarType.ReportIrrelevantGeolocation;
        }

        // 6. Invite new members
        if (settings.InviteMembers && !settings.AddContact && !settings.ReportSpam && !settings.BlockContact)
        {
            return MyTelegram.Messenger.Services.Interfaces.ActionBarType.InviteNewMembers;
        }

        // 7. Join request admin
        if (settings.RequestChatBroadcast && !settings.AddContact && !settings.ReportSpam && !settings.BlockContact)
        {
            return MyTelegram.Messenger.Services.Interfaces.ActionBarType.JoinRequestAdmin;
        }

        // 8. Manage business bot
        if (settings.BlockContact && !settings.AddContact && !settings.ReportSpam)
        {
            return MyTelegram.Messenger.Services.Interfaces.ActionBarType.ManageBusinessBot;
        }

        // 9. Account information
        if (settings.NeedContactsException && !settings.AddContact && !settings.ReportSpam && !settings.BlockContact)
        {
            return MyTelegram.Messenger.Services.Interfaces.ActionBarType.AccountInformation;
        }

        // 10. Bot ads - this would be determined by sponsored messages presence
        // Handled elsewhere as it requires messages.getSponsoredMessages check

        return MyTelegram.Messenger.Services.Interfaces.ActionBarType.None;
    }
}
