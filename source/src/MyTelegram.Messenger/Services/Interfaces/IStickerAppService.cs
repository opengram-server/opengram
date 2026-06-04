namespace MyTelegram.Messenger.Services.Interfaces;

public interface IStickerAppService
{
    Task<long?> GetRandomFeaturedStickerIdAsync(string? emoji = null, CancellationToken cancellationToken = default);
}
