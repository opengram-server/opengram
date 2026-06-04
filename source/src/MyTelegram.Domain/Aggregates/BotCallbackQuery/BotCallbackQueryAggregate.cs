using EventFlow.Aggregates;

namespace MyTelegram.Domain.Aggregates.BotCallbackQuery;

/// <summary>
/// Aggregate for bot callback queries
/// </summary>
public class BotCallbackQueryAggregate : AggregateRoot<BotCallbackQueryAggregate, BotCallbackQueryId>
{
    public BotCallbackQueryAggregate(BotCallbackQueryId id) : base(id)
    {
    }

    public void AnswerCallbackQuery(
        long botUserId,
        long queryId,
        string? text,
        bool showAlert,
        string? url,
        int cacheTime)
    {
        // Emit event for callback query answer
        Emit(new BotCallbackQueryAnsweredEvent(
            botUserId,
            queryId,
            text,
            showAlert,
            url,
            cacheTime));
    }
}

/// <summary>
/// Identity for BotCallbackQueryAggregate
/// </summary>
public class BotCallbackQueryId : Identity<BotCallbackQueryId>
{
    public BotCallbackQueryId(string value) : base(value)
    {
    }
}

/// <summary>
/// Event emitted when callback query is answered
/// </summary>
public class BotCallbackQueryAnsweredEvent : AggregateEvent<BotCallbackQueryAggregate, BotCallbackQueryId>
{
    public BotCallbackQueryAnsweredEvent(
        long botUserId,
        long queryId,
        string? text,
        bool showAlert,
        string? url,
        int cacheTime)
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
