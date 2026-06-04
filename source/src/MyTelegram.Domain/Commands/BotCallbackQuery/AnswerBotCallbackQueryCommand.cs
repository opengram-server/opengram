using EventFlow.Commands;
using MyTelegram.Domain.Aggregates.BotCallbackQuery;

namespace MyTelegram.Domain.Commands.BotCallbackQuery;

/// <summary>
/// Command to answer a bot callback query
/// </summary>
public class AnswerBotCallbackQueryCommand : Command<BotCallbackQueryAggregate, BotCallbackQueryId>
{
    public AnswerBotCallbackQueryCommand(
        BotCallbackQueryId aggregateId,
        long botUserId,
        long queryId,
        string? text,
        bool showAlert,
        string? url,
        int cacheTime) : base(aggregateId)
    {
        BotUserId = botUserId;
        QueryId = queryId;
        Text = text;
        ShowAlert = showAlert;
        Url = url;
        CacheTime = cacheTime;
    }

    public long BotUserId { get; }
    public long QueryId { get; }
    public string? Text { get; }
    public bool ShowAlert { get; }
    public string? Url { get; }
    public int CacheTime { get; }
}
