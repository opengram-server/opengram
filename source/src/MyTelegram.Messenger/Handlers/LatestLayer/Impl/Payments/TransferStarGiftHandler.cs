using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;
using MyTelegram.Queries.StarGift;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/method/payments.transferStarGift" />
///</summary>
internal sealed class TransferStarGiftHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestTransferStarGift, MyTelegram.Schema.IUpdates>,
    Payments.ITransferStarGiftHandler
{
    private readonly ICommandBus _commandBus;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IRandomHelper _randomHelper;
    private readonly Services.IDistributedLockService _lockService;
    private readonly ILogger<TransferStarGiftHandler> _logger;

    public TransferStarGiftHandler(
        ICommandBus commandBus, 
        IQueryProcessor queryProcessor, 
        IRandomHelper randomHelper,
        Services.IDistributedLockService lockService,
        ILogger<TransferStarGiftHandler> logger)
    {
        _commandBus = commandBus;
        _queryProcessor = queryProcessor;
        _randomHelper = randomHelper;
        _lockService = lockService;
        _logger = logger;
    }

    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestTransferStarGift obj)
    {
        string giftInstanceId;
        
        switch (obj.Stargift)
        {
            case TInputSavedStarGiftUser userGift:
                // Загружаем подарок из ReadModel, чтобы получить реальный aggregate ID
                var giftReadModel = await _queryProcessor.ProcessAsync(
                    new GetStarGiftByMessageIdQuery(input.UserId, userGift.MsgId));

                if (giftReadModel == null)
                {
                    throw new RpcException(RpcErrors.RpcErrors400.MessageIdInvalid);
                }

                // У старых подарков нет AggregateId, поэтому возвращаем ошибку
                if (string.IsNullOrEmpty(giftReadModel.AggregateId))
                {
                    Console.WriteLine($"[TransferStarGiftHandler] Gift with Id={giftReadModel.Id} has no AggregateId. This is an old gift that cannot be transferred.");
                    throw new RpcException(RpcErrors.RpcErrors400.MessageIdInvalid);
                }

                giftInstanceId = giftReadModel.AggregateId;
                break;
            case TInputSavedStarGiftChat chatGift:
                // Для подарков в каналах нужно сопоставить ID сообщения с ID подарка.
                // Эту часть, возможно, придётся доработать в зависимости от того, как хранятся подарки каналов.
                 var savedId = await _queryProcessor.ProcessAsync(
                    new GetStarGiftByMessageIdQuery(input.UserId, 0)); // заглушка: нужен полноценный поиск по сообщению
                giftInstanceId = savedId?.AggregateId ?? throw new RpcException(RpcErrors.RpcErrors400.MessageIdInvalid);
                break;
            default:
                throw new RpcException(RpcErrors.RpcErrors400.PeerIdInvalid);
        }
        
        // Распределённая блокировка, чтобы один и тот же подарок нельзя было передать несколько раз
        var lockKey = $"gift:transfer:{giftInstanceId}";
        await using var lockHandle = await _lockService.AcquireLockAsync(lockKey, TimeSpan.FromSeconds(10));

        if (lockHandle == null)
        {
            _logger.LogWarning("[TransferStarGift] Failed to acquire lock for gift {GiftId}", giftInstanceId);
            RpcErrors.RpcErrors400.MessageIdInvalid.ThrowRpcError();
        }

        _logger.LogInformation("[TransferStarGift] Lock acquired for gift {GiftId}", giftInstanceId);

        var aggregateId = StarGiftId.Create(giftInstanceId);
        var transferDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var newOwnerId = obj.ToId.GetPeerId();

        var command = new TransferStarGiftCommand(
            aggregateId,
            input.ToRequestInfo(),
            newOwnerId,
            transferDate
        );

        await _commandBus.PublishAsync(command, CancellationToken.None);

        return new TUpdates
        {
            Updates = new TVector<IUpdate>(),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = transferDate,
            Seq = 0
        };
    }
}
