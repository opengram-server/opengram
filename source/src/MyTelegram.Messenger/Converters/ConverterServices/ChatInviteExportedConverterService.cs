namespace MyTelegram.Messenger.Converters.ConverterServices;

public class ChatInviteExportedConverterService(ILayeredService<IChatInviteExportedConverter> chatInviteExportedLayeredService,
    IChatInviteLinkHelper chatInviteLinkHelper,
    IOptionsMonitor<MyTelegramMessengerServerOptions> options) : IChatInviteExportedConverterService, ITransientDependency
{
    public ILayeredExportedChatInvite ToExportedChatInvite(IChatInviteReadModel readModel, int layer)
    {
        var exportedChatInvite = chatInviteExportedLayeredService.GetConverter(layer).ToExportedChatInvite(readModel);
        exportedChatInvite.Link = chatInviteLinkHelper.GetFullLink(options.CurrentValue.JoinChatDomain, readModel.Link);

        return exportedChatInvite;
    }

    public ILayeredExportedChatInvite ToExportedChatInvite(ChatInviteCreatedEvent source, int layer)
    {
        var exportedChatInvite = chatInviteExportedLayeredService.GetConverter(layer).ToExportedChatInvite(source);
        exportedChatInvite.Link = chatInviteLinkHelper.GetFullLink(options.CurrentValue.JoinChatDomain, source.Hash);

        return exportedChatInvite;
    }

    public ILayeredExportedChatInvite ToExportedChatInvite(ChatInviteEditedEvent source, string link, int layer)
    {
        var exportedChatInvite = chatInviteExportedLayeredService.GetConverter(layer).ToExportedChatInvite(source);
        exportedChatInvite.Link = chatInviteLinkHelper.GetFullLink(options.CurrentValue.JoinChatDomain, link);

        return exportedChatInvite;
    }
}