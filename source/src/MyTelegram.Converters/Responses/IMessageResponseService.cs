using System.Diagnostics.CodeAnalysis;

namespace MyTelegram.Converters.Responses;

public interface IMessageResponseService
{
    ILayeredMessage ToLayeredData(TMessage latestLayerData, int layer);
    ILayeredServiceMessage ToLayeredData(TMessageService latestLayerData, int layer);

    [return: NotNullIfNotNull(nameof(message))]
    IMessage? ToLayeredData(IMessage? message, int layer);
}