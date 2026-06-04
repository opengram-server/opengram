namespace MyTelegram.Converters.Responses;

public class MessageReactionsResponseService(
    ILayeredService<IMessageReactionsResponseConverter> messageReactionsLayeredService)
    : IMessageReactionsResponseService, ITransientDependency
{
    public IMessageReactions? ToLayeredData(IMessageReactions? latestLayerData, int layer)
    {
        switch (latestLayerData)
        {
            case TMessageReactions messageReactions:
                return messageReactionsLayeredService.GetConverter(layer).ToLayeredData(messageReactions);
        }

        return latestLayerData;
    }
}