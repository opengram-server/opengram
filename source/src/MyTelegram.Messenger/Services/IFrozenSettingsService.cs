namespace MyTelegram.Messenger.Services;

public interface IFrozenSettingsService
{
    Task<long?> GetSnowflakeEmojiIdAsync();
    Task RefreshSettingsAsync();
}
