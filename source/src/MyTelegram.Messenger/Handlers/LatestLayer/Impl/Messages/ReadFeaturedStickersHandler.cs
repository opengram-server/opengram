namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Помечает новые популярные (featured) наборы стикеров как прочитанные.
/// See <a href="https://corefork.telegram.org/method/messages.readFeaturedStickers" />
///</summary>
internal sealed class ReadFeaturedStickersHandler :
    RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestReadFeaturedStickers, IBool>,
    Messages.IReadFeaturedStickersHandler
{
    private readonly ILogger<ReadFeaturedStickersHandler> _logger;

    public ReadFeaturedStickersHandler(ILogger<ReadFeaturedStickersHandler> logger)
    {
        _logger = logger;
    }

    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestReadFeaturedStickers obj)
    {
        _logger.LogInformation("ReadFeaturedStickers called - UserId={UserId}, StickerSetIds={Ids}",
            input.UserId, string.Join(",", obj.Id));

        // TODO: сохранять статус прочтения в настройках пользователя или в отдельной ReadModel.
        // Пока просто возвращаем успех.


        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
