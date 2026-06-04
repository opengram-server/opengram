namespace MyTelegram.Messenger.Services.Caching;

public interface IInMemoryCacheLoader
{
    Task LoadAsync();
}