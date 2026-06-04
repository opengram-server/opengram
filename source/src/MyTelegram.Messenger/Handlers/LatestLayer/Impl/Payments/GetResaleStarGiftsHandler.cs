using MyTelegram.Queries.StarGift;
using MyTelegram.Schema.Payments;
using MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Payments;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/method/payments.getResaleStarGifts" />
///</summary>
internal sealed class GetResaleStarGiftsHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestGetResaleStarGifts, MyTelegram.Schema.Payments.IResaleStarGifts>,
    IGetResaleStarGiftsHandler
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly ILogger<GetResaleStarGiftsHandler> _logger;

    public GetResaleStarGiftsHandler(IQueryProcessor queryProcessor, ILogger<GetResaleStarGiftsHandler> logger)
    {
        _queryProcessor = queryProcessor;
        _logger = logger;
    }

    protected override async Task<MyTelegram.Schema.Payments.IResaleStarGifts> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestGetResaleStarGifts obj)
    {
        _logger.LogInformation("GetResaleStarGifts called - UserId={UserId}, GiftId={GiftId}, Offset={Offset}, Limit={Limit}, SortByPrice={SortByPrice}, SortByNum={SortByNum}, Attributes={AttrCount}",
            input.UserId, obj.GiftId, obj.Offset, obj.Limit, obj.SortByPrice, obj.SortByNum, obj.Attributes?.Count ?? 0);

        // Разбираем offset; если пусто или некорректно, используем 0
        var offset = 0;
        if (!string.IsNullOrEmpty(obj.Offset) && int.TryParse(obj.Offset, out var parsedOffset))
        {
            offset = parsedOffset;
        }
        
        var result = await _queryProcessor.ProcessAsync(new GetResaleStarGiftsQuery(
            offset, 
            obj.Limit, 
            obj.GiftId, 
            obj.Attributes, 
            obj.SortByPrice, 
            obj.SortByNum
        ));
        
        _logger.LogInformation("GetResaleStarGifts returning {Count} gifts",
            result is MyTelegram.Schema.Payments.TResaleStarGifts r ? r.Gifts?.Count ?? 0 : 0);
        
        return result;
    }
}
