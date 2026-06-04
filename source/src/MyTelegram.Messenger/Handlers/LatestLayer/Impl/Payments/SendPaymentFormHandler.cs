using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;
using MyTelegram.Schema;
using MyTelegram.Schema.Payments;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// Отправка заполненной платёжной формы.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 MESSAGE_ID_INVALID The provided message id is invalid.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// See <a href="https://corefork.telegram.org/method/payments.sendPaymentForm" />
///</summary>
internal sealed class SendPaymentFormHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestSendPaymentForm, MyTelegram.Schema.Payments.IPaymentResult>,
    Payments.ISendPaymentFormHandler
{
    private readonly ICommandBus _commandBus;
    private readonly IQueryProcessor _queryProcessor;
    private readonly ICacheManager<PaymentFormCacheItem> _cacheManager;
    private readonly ILogger<SendPaymentFormHandler> _logger;

    public SendPaymentFormHandler(ICommandBus commandBus, IQueryProcessor queryProcessor, ICacheManager<PaymentFormCacheItem> cacheManager, ILogger<SendPaymentFormHandler> logger)
    {
        _commandBus = commandBus;
        _queryProcessor = queryProcessor;
        _cacheManager = cacheManager;
        _logger = logger;
    }

    protected override async Task<MyTelegram.Schema.Payments.IPaymentResult> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestSendPaymentForm obj)
    {
        _logger.LogInformation("SendPaymentForm called - UserId={UserId}, FormId={FormId}", input.UserId, obj.FormId);

        var cacheItem = await _cacheManager.GetAsync(obj.FormId.ToString());

        if (cacheItem == null)
        {
            _logger.LogWarning("Payment form expired or not found: FormId={FormId}", obj.FormId);
            throw new RpcException(new RpcError(400, "FORM_EXPIRED"));
        }

        // Проверяем, является ли это передачей подарка
        if (cacheItem.IsTransfer)
        {
            _logger.LogInformation("Processing gift transfer: AggregateId={AggregateId}, ToUserId={ToUserId}",
                cacheItem.AggregateId, cacheItem.ToPeerId);

            // Выполняем передачу
            var aggregateId = StarGiftId.With(cacheItem.AggregateId);
            var transferCommand = new TransferStarGiftCommand(
                aggregateId,
                input.ToRequestInfo(),
                cacheItem.ToPeerId ?? 0,
                (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            );
            
            await _commandBus.PublishAsync(transferCommand, CancellationToken.None);

            _logger.LogInformation("Gift transfer completed");
        }
        // Проверяем, является ли это покупкой с перепродажи
        else if (cacheItem.IsResale)
        {
            _logger.LogInformation("Processing resale purchase: Slug={Slug}, Buyer={BuyerId}, Recipient={RecipientId}",
                cacheItem.ResaleSlug, input.UserId, cacheItem.ToPeerId);

            // Выполняем покупку с перепродажи
            var aggregateId = StarGiftId.With(cacheItem.AggregateId);
            var buyCommand = new BuyStarGiftFromResaleCommand(
                aggregateId,
                input.ToRequestInfo(),
                input.UserId,
                cacheItem.ToPeerId ?? input.UserId, // получатель (или сам покупатель, если дарит себе)
                cacheItem.TotalStars
            );

            await _commandBus.PublishAsync(buyCommand, CancellationToken.None);

            _logger.LogInformation("Resale purchase completed");
        }
        else
        {
            _logger.LogInformation("Processing payment for Gift {GiftId} to Peer {PeerId}", cacheItem.GiftId, cacheItem.ToPeerId);

            // Инициируем отправку подарка Star Gift
            var command = new InitiateStarGiftCommand(
                StarGiftId.Create(Guid.NewGuid().ToString()),
                input.ToRequestInfo(),
                cacheItem.GiftId,
                input.UserId,
                cacheItem.ToPeerId ?? 0, // пока считаем, что пир — пользователь
                cacheItem.ToPeerId,
                0, // MessageId будет назначен сагой или обработчиком
                cacheItem.TotalStars,
                cacheItem.TotalStars, // ConvertStars — обычно равно цене или меньше; логику нужно уточнить
                cacheItem.Message,
                cacheItem.HideName,
                cacheItem.IncludeUpgrade,
                cacheItem.IncludeUpgrade ? 0 : null, // UpgradeStars — логику нужно уточнить
                null, // GiftSticker — берётся в саге или агрегате
                (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            );
            
            await _commandBus.PublishAsync(command, CancellationToken.None);
        }
        
        return new TPaymentResult
        {
            Updates = new TUpdates
            {
                Updates = new TVector<IUpdate>(),
                Users = new TVector<IUser>(),
                Chats = new TVector<IChat>(),
                Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Seq = 0
            }
        };
    }
}
