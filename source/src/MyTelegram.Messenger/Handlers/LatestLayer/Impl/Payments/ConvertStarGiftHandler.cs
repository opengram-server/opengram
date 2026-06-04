using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;
using MyTelegram.Queries.StarGift;
using MyTelegram.Schema;
using MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Payments;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// Convert a <a href="https://corefork.telegram.org/api/gifts">received gift »</a> into Telegram Stars: this will permanently destroy the gift, converting it into <a href="https://corefork.telegram.org/constructor/starGift">starGift</a>.<code>convert_stars</code> <a href="https://corefork.telegram.org/api/stars">Telegram Stars</a>, added to the user's balance.Note that <a href="https://corefork.telegram.org/constructor/starGift">starGift</a>.<code>convert_stars</code> will be less than the buying price (<a href="https://corefork.telegram.org/constructor/starGift">starGift</a>.<code>stars</code>) of the gift if it was originally bought using Telegram Stars bought a long time ago.
/// See <a href="https://corefork.telegram.org/method/payments.convertStarGift" />
///</summary>
internal sealed class ConvertStarGiftHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestConvertStarGift, IBool>,
    IConvertStarGiftHandler
{
    private readonly ICommandBus _commandBus;
    private readonly IQueryProcessor _queryProcessor;
    private readonly Services.IDistributedLockService _lockService;
    private readonly ILogger<ConvertStarGiftHandler> _logger;

    public ConvertStarGiftHandler(
        ICommandBus commandBus, 
        IQueryProcessor queryProcessor,
        Services.IDistributedLockService lockService,
        ILogger<ConvertStarGiftHandler> logger)
    {
        _commandBus = commandBus;
        _queryProcessor = queryProcessor;
        _lockService = lockService;
        _logger = logger;
    }

    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestConvertStarGift obj)
    {
        string giftInstanceId;

        // Сначала получаем идентификатор подарка, чтобы взять блокировку.
        // Определяем идентификатор по типу входных данных.
        switch (obj.Stargift)
        {
            case TInputSavedStarGiftUser userGift:
                // Загружаем подарок из ReadModel, чтобы получить настоящий aggregate id
                var giftReadModel = await _queryProcessor.ProcessAsync(
                    new GetStarGiftByMessageIdQuery(input.UserId, userGift.MsgId));
                
                if (giftReadModel == null)
                {
                    throw new RpcException(RpcErrors.RpcErrors400.MessageIdInvalid);
                }
                
                giftInstanceId = giftReadModel.Id;
                break;
            case TInputSavedStarGiftChat chatGift:
                var gift = await _queryProcessor.ProcessAsync(
                    new GetStarGiftByMessageIdQuery(input.UserId, 0));
                giftInstanceId = gift?.Id ?? throw new RpcException(RpcErrors.RpcErrors400.MessageIdInvalid);
                break;
            default:
                throw new RpcException(RpcErrors.RpcErrors400.PeerIdInvalid);
        }
        
        // Распределённая блокировка, чтобы один и тот же подарок нельзя было конвертировать несколько раз
        var lockKey = $"gift:convert:{giftInstanceId}";
        await using var lockHandle = await _lockService.AcquireLockAsync(lockKey, TimeSpan.FromSeconds(10));

        if (lockHandle == null)
        {
            _logger.LogWarning("[ConvertStarGift] Failed to acquire lock for gift {GiftId}", giftInstanceId);
            RpcErrors.RpcErrors400.MessageIdInvalid.ThrowRpcError();
        }

        _logger.LogInformation("[ConvertStarGift] Lock acquired for gift {GiftId}", giftInstanceId);

        var aggregateId = StarGiftId.Create(giftInstanceId);
        var convertDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var command = new ConvertStarGiftCommand(
            aggregateId,
            input.ToRequestInfo(),
            convertDate
        );

        await _commandBus.PublishAsync(command, CancellationToken.None);

        // TODO: Add stars to user's balance
        // This would require integration with the Stars/Payment system

        return new TBoolTrue();
    }
}
