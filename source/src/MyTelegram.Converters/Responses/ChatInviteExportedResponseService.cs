namespace MyTelegram.Converters.Responses;

public class ChatInviteExportedResponseService(
    ILayeredService<IChatInviteExportedResponseConverter> chatInviteExportedLayeredService)
    : IChatInviteExportedResponseService, ITransientDependency
{
    public IExportedChatInvite? ToLayeredData(IExportedChatInvite? latestLayerData, int layer)
    {
        switch (latestLayerData)
        {
            case TChatInviteExported chatInviteExported:
                return chatInviteExportedLayeredService.GetConverter(layer).ToLayeredData(chatInviteExported);
        }

        return latestLayerData;
    }
}