using MyTelegram.Queries.StarGift;
using MyTelegram.Schema.Payments;
using MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Payments;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

/// <summary>
/// Возвращает подарки, закреплённые в профиле указанного пользователя,
/// либо все подарки, полученные текущим пользователем.
/// </summary>
public class GetUserStarGiftsHandler : RpcResultObjectHandler<RequestGetUserStarGifts, MyTelegram.Schema.Payments.IUserStarGifts>,
    IGetUserStarGiftsHandler
{
    private readonly IObjectMessageSender _objectMessageSender;
    private readonly IQueryProcessor _queryProcessor;

    public GetUserStarGiftsHandler(IObjectMessageSender objectMessageSender, IQueryProcessor queryProcessor)
    {
        _objectMessageSender = objectMessageSender;
        _queryProcessor = queryProcessor;
    }

    protected override async Task<MyTelegram.Schema.Payments.IUserStarGifts> HandleCoreAsync(IRequestInput input,
        RequestGetUserStarGifts obj)
    {
        // Определяем идентификатор пользователя; если он не задан, берём текущего
        long targetUserId;
        if (obj.UserId != null)
        {
            // Преобразуем input user в идентификатор пользователя
            var inputUser = obj.UserId as TInputUser;
            if (inputUser != null)
            {
                targetUserId = inputUser.UserId;
            }
            else if (obj.UserId is TInputUserSelf)
            {
                targetUserId = input.UserId;
            }
            else
            {
                // По умолчанию используем текущего пользователя
                targetUserId = input.UserId;
            }
        }
        else
        {
            targetUserId = input.UserId;
        }

        // Разбираем смещение (offset)
        string offset = obj.Offset ?? "";

        Console.WriteLine($"[GetUserStarGiftsHandler] ===== REQUEST RECEIVED =====");
        Console.WriteLine($"[GetUserStarGiftsHandler] RequestingUserId={input.UserId}, TargetUserId={targetUserId}, Offset={offset}, Limit={obj.Limit}");
        var result = await _queryProcessor.ProcessAsync(new GetUserStarGiftsQuery(targetUserId, offset, obj.Limit));

        if (result is TUserStarGifts concrete)
        {
            Console.WriteLine($"[GetUserStarGiftsHandler] Returned {concrete.Count} gifts");
        }
        
        return result;
    }
}
