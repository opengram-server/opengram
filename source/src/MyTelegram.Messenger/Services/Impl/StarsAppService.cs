using Microsoft.Extensions.Logging;
using MyTelegram.Schema;
using MyTelegram.Messenger.Services;
using MyTelegram.Messenger.Services.Interfaces;
using MyTelegram.Domain.Shared;
using EventFlow.Commands;
using EventFlow.Aggregates.ExecutionResults;
using MyTelegram.Domain.Commands;

namespace MyTelegram.Messenger.Services.Impl;

/// <summary>
/// Interface for managing Telegram Stars functionality
/// </summary>
public interface IStarsAppService
{
    Task<StarsStatus> GetStarsStatusAsync(long peerId, bool isTon = false);
    Task<StarsStatus> GetStarsTransactionsAsync(GetStarsTransactionsQueryOld query);
    Task<StarsRevenueStats> GetStarsRevenueStatsAsync(long peerId, bool isDarkTheme = false, bool isTon = false);
    Task<List<StarsTopupOption>> GetStarsTopupOptionsAsync();
    Task<List<StarsGiftOption>> GetStarsGiftOptionsAsync(long? userId = null);
    Task<StarsStatus> GetStarsSubscriptionsAsync(long peerId, string? offset = null, int limit = 50);
    Task<bool> UpdateStarsBalanceAsync(long peerId, long amount, string transactionType, string? description = null);
    Task<ServicesStarsTransaction?> CreateTransactionAsync(CreateStarsTransactionRequest request);
}

/// <summary>
/// Service for managing Telegram Stars functionality
/// </summary>
internal sealed class StarsAppService(
    ILogger<StarsAppService> logger,
    IQueryProcessor queryProcessor,
    ICommandBus commandBus) : IStarsAppService
{
    public async Task<StarsStatus> GetStarsStatusAsync(long peerId, bool isTon = false)
    {
        logger.LogInformation("Getting stars status for peer {PeerId}, isTon: {IsTon}", peerId, isTon);

        var query = new GetStarsStatusQueryOld(peerId, isTon);
        var result = await queryProcessor.ProcessAsync(query);

        if (result == null)
        {
            logger.LogWarning("No stars status found for peer {PeerId}", peerId);
            return CreateEmptyStarsStatus();
        }

        logger.LogInformation("Retrieved stars status: balance {Balance}, subscriptions {SubscriptionCount}", 
            result.Balance, result.Subscriptions?.Count ?? 0);

        return ConvertToStarsStatus(result);
    }

    public async Task<StarsStatus> GetStarsTransactionsAsync(GetStarsTransactionsQueryOld query)
    {
        logger.LogInformation("Getting stars transactions for peer {PeerId}, filters: inbound={Inbound}, outbound={Outbound}, ton={Ton}", 
            query.PeerId, query.IsInbound, query.IsOutbound, query.IsTon);

        var result = await queryProcessor.ProcessAsync(query);

        if (result == null)
        {
            return CreateEmptyStarsStatus();
        }

        logger.LogInformation("Retrieved {TransactionCount} stars transactions for peer {PeerId}", 
            result.History?.Count ?? 0, query.PeerId);

        return ConvertToStarsStatus(result);
    }

    public async Task<StarsRevenueStats> GetStarsRevenueStatsAsync(long peerId, bool isDarkTheme = false, bool isTon = false)
    {
        logger.LogInformation("Getting stars revenue stats for peer {PeerId}, dark: {Dark}, ton: {Ton}", 
            peerId, isDarkTheme, isTon);

        var query = new GetStarsRevenueStatsQuery(peerId, isDarkTheme, isTon);
        var result = await queryProcessor.ProcessAsync(query);

        if (result == null)
        {
            return CreateEmptyRevenueStats();
        }

        logger.LogInformation("Retrieved revenue stats for peer {PeerId}", peerId);

        return result;
    }

    public async Task<List<StarsTopupOption>> GetStarsTopupOptionsAsync()
    {
        logger.LogInformation("Getting stars topup options");

        var query = new GetStarsTopupOptionsQuery();
        var result = await queryProcessor.ProcessAsync(query);

        var options = result?.ToList() ?? new List<StarsTopupOption>();
        logger.LogInformation("Retrieved {Count} stars topup options", options.Count);

        return options;
    }

    public async Task<List<StarsGiftOption>> GetStarsGiftOptionsAsync(long? userId = null)
    {
        logger.LogInformation("Getting stars gift options for user {UserId}", userId);

        var query = new GetStarsGiftOptionsQuery { UserId = userId };
        var result = await queryProcessor.ProcessAsync(query);

        var options = result?.ToList() ?? new List<StarsGiftOption>();
        logger.LogInformation("Retrieved {Count} stars gift options", options.Count);

        return options;
    }

    public async Task<StarsStatus> GetStarsSubscriptionsAsync(long peerId, string? offset = null, int limit = 50)
    {
        logger.LogInformation("Getting stars subscriptions for peer {PeerId}, offset: {Offset}, limit: {Limit}", 
            peerId, offset, limit);

        const int maxLimit = 100;
        var actualLimit = Math.Min(limit, maxLimit);

        var query = new GetStarsSubscriptionsQuery
        {
            PeerId = peerId,
            Offset = offset ?? string.Empty,
            Limit = actualLimit
        };

        var result = await queryProcessor.ProcessAsync(query);

        if (result == null)
        {
            return CreateEmptyStarsStatus();
        }

        logger.LogInformation("Retrieved stars status for peer {PeerId}", peerId);

        return result;
    }

    public async Task<bool> UpdateStarsBalanceAsync(long peerId, long amount, string transactionType, string? description = null)
    {
        logger.LogInformation("Updating stars balance for peer {PeerId}, amount: {Amount}, type: {Type}", 
            peerId, amount, transactionType);

        var command = new UpdateStarsBalanceCommand(
            new UserId(peerId.ToString()), 
            new MyTelegram.RequestInfo(0, peerId, 0, 0, 0, Guid.NewGuid(), 0, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
            amount,
            transactionType,
            description ?? string.Empty);

        await commandBus.PublishAsync<UserAggregate, UserId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Stars balance update command published for peer {PeerId}", peerId);
        return true;
    }

    public async Task<ServicesStarsTransaction?> CreateTransactionAsync(CreateStarsTransactionRequest request)
    {
        logger.LogInformation("Creating stars transaction for peer {PeerId}, amount: {Amount}", 
            request.PeerId, request.Amount);

        var command = new CreateStarsTransactionCommand(
            new UserId(request.PeerId.ToString()), 
            new MyTelegram.RequestInfo(0, request.PeerId, 0, 0, 0, Guid.NewGuid(), 0, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
            request.Amount,
            request.Peer,
            request.Title ?? string.Empty,
            request.Description ?? string.Empty,
            request.TransactionType);

        await commandBus.PublishAsync<UserAggregate, UserId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Stars transaction creation command published for peer {PeerId}", request.PeerId);
        
        // Return the created transaction (would need to query back from database in real implementation)
        return new ServicesStarsTransaction
        {
            Id = Guid.NewGuid().ToString(),
            UserId = request.PeerId,
            Amount = request.Amount,
            Type = Enum.Parse<StarsTransactionType>(request.TransactionType),
            CreatedAt = DateTime.UtcNow,
            Description = request.Description,
            SourceId = request.Peer?.ToString()
        };
    }

    private static StarsStatus CreateEmptyStarsStatus()
    {
        return new StarsStatus
        {
            Balance = 0,
            IsBot = false,
            NextOffset = DateTime.UtcNow
        };
    }

    private static StarsStatus ConvertToStarsStatus(ServicesStarsStatus servicesStatus)
    {
        return new StarsStatus
        {
            Balance = servicesStatus.Balance,
            IsBot = false,
            NextOffset = servicesStatus.LastUpdated
        };
    }

    private static StarsRevenueStats CreateEmptyRevenueStats()
    {
        return new StarsRevenueStats
        {
            TotalRevenue = 0,
            DailyRevenue = 0,
            WeeklyRevenue = 0,
            MonthlyRevenue = 0,
            From = DateTime.UtcNow.AddDays(-30),
            To = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Request object for creating a stars transaction
/// </summary>
public class CreateStarsTransactionRequest
{
    public long PeerId { get; set; }
    public long Amount { get; set; }
    public StarsTransactionPeer Peer { get; set; } = new();
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string TransactionType { get; set; } = string.Empty;
}

/// <summary>
/// Query object for getting stars topup options
/// </summary>
public class GetStarsTopupOptionsQuery : IQuery<List<StarsTopupOption>>
{
}

/// <summary>
/// Query object for getting stars gift options
/// </summary>
public class GetStarsGiftOptionsQuery : IQuery<List<StarsGiftOption>>
{
    public long? UserId { get; set; }
}

/// <summary>
/// Query object for getting stars subscriptions
/// </summary>
public class GetStarsSubscriptionsQuery : IQuery<StarsStatus>
{
    public long PeerId { get; set; }
    public string Offset { get; set; } = string.Empty;
    public int Limit { get; set; }
}

/// <summary>
/// Command for updating stars balance
/// </summary>
public class UpdateStarsBalanceCommand(
    UserId aggregateId,
    MyTelegram.RequestInfo requestInfo,
    long amount,
    string transactionType,
    string description) : RequestCommand2<UserAggregate, UserId, IExecutionResult>(aggregateId, requestInfo)
{
    public long Amount { get; } = amount;
    public string TransactionType { get; } = transactionType;
    public string Description { get; } = description;
}

/// <summary>
/// Command for creating stars transaction
/// </summary>
public class CreateStarsTransactionCommand(
    UserId aggregateId,
    MyTelegram.RequestInfo requestInfo,
    long amount,
    StarsTransactionPeer peer,
    string title,
    string description,
    string transactionType) : RequestCommand2<UserAggregate, UserId, IExecutionResult>(aggregateId, requestInfo)
{
    public long Amount { get; } = amount;
    public StarsTransactionPeer Peer { get; } = peer;
    public string Title { get; } = title;
    public string Description { get; } = description;
    public string TransactionType { get; } = transactionType;
}
