using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;
using MyTelegram.Queries.StarGift;
using MyTelegram.Schema;
using MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Payments;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// Display or remove a <a href="https://corefork.telegram.org/api/gifts">received gift »</a> from our profile.
/// See <a href="https://corefork.telegram.org/method/payments.saveStarGift" />
///</summary>
internal sealed class SaveStarGiftHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestSaveStarGift, IBool>,
    ISaveStarGiftHandler
{
    private readonly ICommandBus _commandBus;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IRandomHelper _randomHelper;

    public SaveStarGiftHandler(ICommandBus commandBus, IQueryProcessor queryProcessor, IRandomHelper randomHelper)
    {
        _commandBus = commandBus;
        _queryProcessor = queryProcessor;
        _randomHelper = randomHelper;
    }

    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestSaveStarGift obj)
    {
        string giftInstanceId;

        // Определяем идентификатор подарка по типу входных данных
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
                    Console.WriteLine($"[SaveStarGiftHandler] Gift with Id={giftReadModel.Id} has no AggregateId. This is an old gift that cannot be saved/unsaved.");
                    throw new RpcException(RpcErrors.RpcErrors400.MessageIdInvalid);
                }

                giftInstanceId = giftReadModel.AggregateId;
                break;
            case TInputSavedStarGiftChat chatGift:
                // Для подарков в каналах используем saved_id, если он есть
                var savedId = await _queryProcessor.ProcessAsync(
                    new GetStarGiftByMessageIdQuery(input.UserId, 0)); // нужен полноценный поиск по сообщению
                giftInstanceId = savedId?.AggregateId ?? throw new RpcException(RpcErrors.RpcErrors400.MessageIdInvalid);
                break;
            case TInputSavedStarGiftSlug slugGift:
                // Поиск подарка по slug (реализация зависит от того, как генерируется slug)
                throw new NotImplementedException("Slug-based gift lookup not implemented");
            default:
                throw new RpcException(RpcErrors.RpcErrors400.PeerIdInvalid);
        }

        var aggregateId = StarGiftId.Create(giftInstanceId);
        var save = !obj.Unsave;
        var savedIdValue = save ? _randomHelper.NextInt64() : (long?)null;

        var command = new SaveStarGiftCommand(
            aggregateId,
            input.ToRequestInfo(),
            save,
            savedIdValue
        );

        await _commandBus.PublishAsync(command, CancellationToken.None);

        return new TBoolTrue();
    }
}
