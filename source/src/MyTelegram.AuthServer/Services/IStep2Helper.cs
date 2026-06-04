namespace MyTelegram.AuthServer.Services;

public interface IStep2Helper
{
    Task<Step2Output> GetServerDhParamsAsync(RequestReqDHParams req);
}