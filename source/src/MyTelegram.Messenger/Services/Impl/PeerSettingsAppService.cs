using MyTelegram.Schema;
using MyTelegram.Messenger.Services.Interfaces;

namespace MyTelegram.Messenger.Services.Impl;

public class PeerSettingsAppService(IQueryProcessor queryProcessor, IPeerHelper peerHelper) : IPeerSettingsAppService, ITransientDependency
{
    public async Task<MyTelegram.PeerSettings> GetAsync(long userId,
        Peer peer)
    {
        if (userId == peer.PeerId || peerHelper.IsBotUser(userId))
        {
            return new MyTelegram.PeerSettings();
        }

        var peerSettingsReadModel = await queryProcessor.ProcessAsync(new GetPeerSettingsQuery(userId, peer.PeerId));
        if (peerSettingsReadModel != null)
        {
            if (peerSettingsReadModel.HiddenPeerSettingsBar)
            {
                return new MyTelegram.PeerSettings();
            }

            if (peerSettingsReadModel.PeerSettings != null)
            {
                return new MyTelegram.PeerSettings
                {
                    AddContact = peerSettingsReadModel.PeerSettings.AddContact,
                    BlockContact = peerSettingsReadModel.PeerSettings.BlockContact,
                    NeedContactsException = peerSettingsReadModel.PeerSettings.NeedContactsException,
                    ReportGeo = peerSettingsReadModel.PeerSettings.ReportGeo,
                    ReportSpam = peerSettingsReadModel.PeerSettings.ReportSpam,
                    ShareContact = peerSettingsReadModel.PeerSettings.ShareContact,
                    Autoarchived = peerSettingsReadModel.PeerSettings.Autoarchived,
                    InviteMembers = peerSettingsReadModel.PeerSettings.InviteMembers,
                    RequestChatBroadcast = peerSettingsReadModel.PeerSettings.RequestChatBroadcast,
                    BusinessBotPaused = peerSettingsReadModel.PeerSettings.BusinessBotPaused,
                    BusinessBotCanReply = peerSettingsReadModel.PeerSettings.BusinessBotCanReply,
                    GeoDistance = peerSettingsReadModel.PeerSettings.GeoDistance,
                    RequestChatTitle = peerSettingsReadModel.PeerSettings.RequestChatTitle,
                    RequestChatDate = peerSettingsReadModel.PeerSettings.RequestChatDate,
                    BusinessBotId = peerSettingsReadModel.PeerSettings.BusinessBotId,
                    BusinessBotManageUrl = peerSettingsReadModel.PeerSettings.BusinessBotManageUrl,
                    ChargePaidMessageStars = peerSettingsReadModel.PeerSettings.ChargePaidMessageStars,
                    RegistrationMonth = peerSettingsReadModel.PeerSettings.RegistrationMonth,
                    PhoneCountry = peerSettingsReadModel.PeerSettings.PhoneCountry,
                    NameChangeDate = peerSettingsReadModel.PeerSettings.NameChangeDate,
                    PhotoChangeDate = peerSettingsReadModel.PeerSettings.PhotoChangeDate,
                };
            }

            return new MyTelegram.PeerSettings();
        }

        return new MyTelegram.PeerSettings();
    }

    public Task<IPeerSettingsReadModel?> GetPeerSettingsAsync(long userId, long peerId)
    {
        if (userId == peerId || peerHelper.IsBotUser(peerId))
        {
            return Task.FromResult<IPeerSettingsReadModel?>(null);
        }
        return queryProcessor.ProcessAsync(new GetPeerSettingsQuery(userId, peerId));
    }

    public Task<List<MyTelegram.PeerSettings>> GetPeerSettingsListAsync(GetPeerSettingsListInput input)
    {
        return Task.FromResult(new List<MyTelegram.PeerSettings>());
    }
}