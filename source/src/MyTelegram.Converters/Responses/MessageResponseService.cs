using System.Diagnostics.CodeAnalysis;

//using TUpdateDeleteScheduledMessages = MyTelegram.Schema.Layer191.TUpdateDeleteScheduledMessages;
//using TUpdateGroupCall = MyTelegram.Schema.Layer195.TUpdateGroupCall;
//using TUpdateMessageExtendedMedia = MyTelegram.Schema.Layer182.TUpdateMessageExtendedMedia;
//using TUpdateStarsBalance = MyTelegram.Schema.Layer192.TUpdateStarsBalance;

namespace MyTelegram.Converters.Responses;

public class MessageResponseService(
    IMessageMediaResponseService messageMediaResponseService,
    IMessageReactionsResponseService messageReactionsResponseService,
    ILayeredService<IMessageResponseConverter> messageResponseLayeredService,
    ILayeredService<IMessageServiceResponseConverter> messageServiceLayeredService)
    : IMessageResponseService, ITransientDependency
{
    public ILayeredMessage ToLayeredData(TMessage latestLayerData, int layer)
    {
        var media = messageMediaResponseService.ToLayeredData(latestLayerData.Media, layer);
        var targetLayerData = messageResponseLayeredService.GetConverter(layer).ToLayeredData(latestLayerData);
        targetLayerData.Reactions = messageReactionsResponseService.ToLayeredData(targetLayerData.Reactions, layer);
        targetLayerData.Media = media;

        return targetLayerData;
    }

    public ILayeredServiceMessage ToLayeredData(TMessageService latestLayerData, int layer)
    {
        var targetLayerData = messageServiceLayeredService.GetConverter(layer).ToLayeredData(latestLayerData);
        targetLayerData.Reactions = messageReactionsResponseService.ToLayeredData(targetLayerData.Reactions, layer);

        return targetLayerData;
    }

    [return: NotNullIfNotNull(nameof(message))]
    public IMessage? ToLayeredData(IMessage? message, int layer)
    {
        switch (message)
        {
            case TMessage tMessage:
                return ToLayeredData(tMessage, layer);
            case TMessageService tMessageService:
                return ToLayeredData(tMessageService, layer);
        }

        return message;
    }
}