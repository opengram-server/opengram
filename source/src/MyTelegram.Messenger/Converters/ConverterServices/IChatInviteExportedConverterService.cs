namespace MyTelegram.Messenger.Converters.ConverterServices;

public interface IChatInviteExportedConverterService
{
    ILayeredExportedChatInvite ToExportedChatInvite(IChatInviteReadModel readModel, int layer);
    ILayeredExportedChatInvite ToExportedChatInvite(ChatInviteCreatedEvent source, int layer);
    ILayeredExportedChatInvite ToExportedChatInvite(ChatInviteEditedEvent source, string link, int layer);
}