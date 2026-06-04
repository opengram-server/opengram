namespace MyTelegram.Messenger.Converters.TLObjects.Interfaces;
public interface IChatInviteExportedConverter : ILayeredConverter
{
    ILayeredExportedChatInvite ToExportedChatInvite(IChatInviteReadModel readModel);
    ILayeredExportedChatInvite ToExportedChatInvite(ChatInviteCreatedEvent source);
    ILayeredExportedChatInvite ToExportedChatInvite(ChatInviteEditedEvent source);
}