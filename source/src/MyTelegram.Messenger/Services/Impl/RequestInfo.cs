namespace MyTelegram.Messenger.Services.Impl;

public class RequestInfo
{
    public long UserId { get; set; }
    public string RequestId { get; set; } = string.Empty;

    public RequestInfo(long userId, string requestId)
    {
        UserId = userId;
        RequestId = requestId;
    }
}
