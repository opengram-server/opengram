namespace MyTelegram.AuthServer.Services;

public interface IStep3Helper
{
    Task<Step3Output> SetClientDhParamsAnswerAsync(RequestSetClientDHParams req);
}