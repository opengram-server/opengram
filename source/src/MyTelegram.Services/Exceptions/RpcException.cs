using MyTelegram.Schema;

namespace MyTelegram.Services.Exceptions;

public class RpcException(int errorCode, string errorMessage) : Exception(errorMessage)
{
    public TRpcError Error { get; set; } = new()
    {
        ErrorCode = errorCode,
        ErrorMessage = errorMessage
    };
    //public long RequestMessageId { get; }

    //RequestMessageId = requestMessageId;
}