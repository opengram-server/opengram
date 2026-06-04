using EventFlow.Commands;
using MyTelegram.Domain.Aggregates.BotCallbackQuery;
using MyTelegram.Domain.Commands.BotCallbackQuery;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Set the callback answer to a user button press (bots only)
/// <para>Possible errors</para>
/// Code Type Description
/// 400 MESSAGE_TOO_LONG The provided message is too long.
/// 400 QUERY_ID_INVALID The query ID is invalid.
/// 400 URL_INVALID Invalid URL provided.
/// 400 USER_BOT_REQUIRED This method can only be called by a bot.
/// See <a href="https://corefork.telegram.org/method/messages.setBotCallbackAnswer" />
///</summary>
internal sealed class SetBotCallbackAnswerHandler(
    ICommandBus commandBus,
    ILogger<SetBotCallbackAnswerHandler> logger) 
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSetBotCallbackAnswer, IBool>,
    Messages.ISetBotCallbackAnswerHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestSetBotCallbackAnswer obj)
    {
        logger.LogInformation("Bot {UserId} answering callback query {QueryId}", 
            input.UserId, obj.QueryId);

        // Validate message length
        if (obj.Message != null && obj.Message.Length > 200)
        {
            RpcErrors.RpcErrors400.MessageTooLong.ThrowRpcError();
        }

        // Validate URL
        if (obj.Url != null && !Uri.IsWellFormedUriString(obj.Url, UriKind.Absolute))
        {
            RpcErrors.RpcErrors400.UrlInvalid.ThrowRpcError();
        }

        // Create command to answer callback query
        var command = new AnswerBotCallbackQueryCommand(
            BotCallbackQueryId.NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"callback-{obj.QueryId}"),
            input.UserId,
            obj.QueryId,
            obj.Message,
            obj.Alert,
            obj.Url,
            obj.CacheTime);

        await commandBus.PublishAsync(command, CancellationToken.None);

        return new TBoolTrue();
    }
}
