namespace MyTelegram.Messenger.Services;

public interface IFrozenIconService
{
    Task<string> GetFrozenIconAsync();
}
