// ReSharper disable All

using MyTelegram.Queries;
using MyTelegram.ReadModel.Interfaces;
using MyTelegram.Schema;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Auth;

///<summary>
/// Login as a bot
/// <para>Possible errors</para>
/// Code Type Description
/// 400 ACCESS_TOKEN_EXPIRED Access token expired.
/// 400 ACCESS_TOKEN_INVALID Access token invalid.
/// 400 API_ID_INVALID API ID invalid.
/// 400 API_ID_PUBLISHED_FLOOD This API id was published somewhere, you can't use it now.
/// See <a href="https://corefork.telegram.org/method/auth.importBotAuthorization" />
///</summary>
internal sealed class ImportBotAuthorizationHandler(
    IQueryProcessor queryProcessor,
    ILogger<ImportBotAuthorizationHandler> logger,
    IObjectMapper objectMapper) 
    : RpcResultObjectHandler<Schema.Auth.RequestImportBotAuthorization, Schema.Auth.IAuthorization>,
    IImportBotAuthorizationHandler
{
    protected override async Task<Schema.Auth.IAuthorization> HandleCoreAsync(IRequestInput input,
        Schema.Auth.RequestImportBotAuthorization obj)
    {
        // Находим бота по токену
        var botReadModel = await queryProcessor.ProcessAsync(
            new GetBotByTokenQuery(obj.BotAuthToken),
            CancellationToken.None
        );

        if (botReadModel == null)
        {
            logger.LogWarning("Bot authorization failed: Invalid token");
            RpcErrors.RpcErrors400.AccessTokenInvalid.ThrowRpcError();
        }

        logger.LogInformation("Bot authorized successfully: UserId={UserId}", botReadModel.UserId);

        // Загружаем данные пользователя-бота
        var userReadModel = await queryProcessor.ProcessAsync(
            new GetUserByIdQuery(botReadModel.UserId),
            CancellationToken.None
        );

        if (userReadModel == null)
        {
            logger.LogError("Bot user not found: UserId={UserId}", botReadModel.UserId);
            RpcErrors.RpcErrors400.UserIdInvalid.ThrowRpcError();
        }

        // Преобразуем в TUser
        var user = objectMapper.Map<IUserReadModel, IUser>(userReadModel);

        // Возвращаем результат авторизации
        return new Schema.Auth.TAuthorization
        {
            User = user
        };
    }
}
