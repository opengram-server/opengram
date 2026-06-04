namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class PeerSettingsConverter(IObjectMapper objectMapper, IPeerHelper peerHelper)
    : IPeerSettingsConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IPeerSettings ToPeerSettings(long selfUserId, long targetUserId, IPeerSettingsReadModel? readModel,
        ContactType? contactType)
    {
        if (targetUserId == MyTelegramConsts.OfficialUserId || selfUserId == targetUserId ||
            peerHelper.IsBotUser(targetUserId))
        {
            return new TPeerSettings();
        }

        var isContact = contactType == ContactType.Mutual || contactType == ContactType.TargetUserIsMyContact;

        var settings = new TPeerSettings
        {
            ShareContact = contactType == ContactType.ContactOfTargetUser
        };

        if (readModel == null)
        {
            // Default behavior for new chats - determine action bar type based on contact status
            if (!isContact)
            {
                // Report spam, block or add contact bar
                settings.ReportSpam = true;
                settings.AddContact = true;
                settings.BlockContact = true;
            }
            else
            {
                // Already a contact - no action bar needed
                settings.ReportSpam = false;
                settings.AddContact = false;
                settings.BlockContact = false;
            }

            return settings;
        }

        if (readModel.PeerSettings != null)
        {
            settings = objectMapper.Map<PeerSettings, TPeerSettings>(readModel.PeerSettings);

            if (!readModel.HiddenPeerSettingsBar && !isContact)
            {
                // Only show action bar if not hidden and not a contact
                if (!settings.ReportSpam && !settings.AddContact && !settings.BlockContact)
                {
                    // Default to report spam bar if no other flags are set
                    settings.ReportSpam = true;
                    settings.AddContact = false;
                    settings.BlockContact = false;
                }
            }
            else if (isContact)
            {
                // Clear action bar flags for contacts
                settings.ReportSpam = false;
                settings.AddContact = false;
                settings.BlockContact = false;
            }
        }

        return settings;
    }
}