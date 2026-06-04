namespace MyTelegram.Converters.Responses;

public interface IMessageReactionsResponseService
{
    IMessageReactions? ToLayeredData(IMessageReactions? latestLayerData, int layer);
}