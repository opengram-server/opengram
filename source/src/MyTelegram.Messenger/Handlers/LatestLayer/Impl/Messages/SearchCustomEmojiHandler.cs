using Microsoft.Extensions.Logging;
using MyTelegram.Queries;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Look for <a href="https://corefork.telegram.org/api/custom-emoji">custom emojis</a> associated to a UTF8 emoji
/// <para>Possible errors</para>
/// Code Type Description
/// 400 EMOTICON_EMPTY The emoji is empty.
/// See <a href="https://corefork.telegram.org/method/messages.searchCustomEmoji" />
///</summary>
internal sealed class SearchCustomEmojiHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSearchCustomEmoji, MyTelegram.Schema.IEmojiList>,
    Messages.ISearchCustomEmojiHandler
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly ILogger<SearchCustomEmojiHandler> _logger;

    public SearchCustomEmojiHandler(
        IQueryProcessor queryProcessor,
        ILogger<SearchCustomEmojiHandler> logger)
    {
        _queryProcessor = queryProcessor;
        _logger = logger;
    }

    protected override async Task<MyTelegram.Schema.IEmojiList> HandleCoreAsync(
        IRequestInput input,
        MyTelegram.Schema.Messages.RequestSearchCustomEmoji obj)
    {
        // Валидация: emoticon не может быть пустым
        if (string.IsNullOrWhiteSpace(obj.Emoticon))
        {
            _logger.LogWarning("User {UserId} provided empty emoticon", input.UserId);
            RpcErrors.RpcErrors400.EmoticonEmpty.ThrowRpcError();
        }

        _logger.LogInformation(
            "User {UserId} searching custom emoji for emoticon '{Emoticon}'",
            input.UserId,
            obj.Emoticon);

        // TODO: Implement full custom emoji search
        _logger.LogWarning("SearchCustomEmoji not fully implemented yet");
        
        // Возвращаем пустой результат
        return new TEmojiList
        {
            Hash = 0,
            DocumentId = new TVector<long>()
        };
    }

}
