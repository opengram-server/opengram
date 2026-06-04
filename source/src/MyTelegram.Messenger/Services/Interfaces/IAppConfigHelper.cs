namespace MyTelegram.Messenger.Services.Interfaces;
public interface IAppConfigHelper
{
    IJSONValue GetAppConfig();

    int GetAppConfigHash();
}
