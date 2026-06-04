namespace MyTelegram.Converters.Responses;

public interface IChatInviteExportedResponseService
{
    IExportedChatInvite? ToLayeredData(IExportedChatInvite? latestLayerData, int layer);
}