using Moq;
using Microsoft.Extensions.Logging;
using MyTelegram.EventFlow.ReadStores;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;
using MyTelegram.TestBase;
using Shouldly;
using Xunit;

namespace MyTelegram.Domain.Tests.UnitTests.QueryHandlers;

public class GetBotsByOwnerUserIdQueryHandlerTests : MyTelegramTestBase
{
    private const long TestOwnerUserId = 100;
    private const long TestBotUserId1 = 201;
    private const long TestBotUserId2 = 202;

    private Mock<IQueryOnlyReadModelStore<BotReadModel>> CreateStoreMock(
        IReadOnlyCollection<BotReadModel>? bots = null)
    {
        var storeMock = new Mock<IQueryOnlyReadModelStore<BotReadModel>>();
        storeMock
            .Setup(s => s.FindAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<BotReadModel, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOptions<BotReadModel>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(bots ?? Array.Empty<BotReadModel>());
        return storeMock;
    }

    [Fact]
    public async Task ExecuteQueryAsync_ReturnsBotsOwnedByUser()
    {
        // Arrange
        var ownedBots = new List<BotReadModel>
        {
            new(TestBotUserId1, TestOwnerUserId, "token1", "Bot1", "bot1"),
            new(TestBotUserId2, TestOwnerUserId, "token2", "Bot2", "bot2")
        };
        var store = CreateStoreMock(ownedBots);
        var logger = new Mock<ILogger<MyTelegram.QueryHandlers.MongoDB.Bot.GetBotsByOwnerUserIdQueryHandler>>();
        var handler = new MyTelegram.QueryHandlers.MongoDB.Bot.GetBotsByOwnerUserIdQueryHandler(store.Object, logger.Object);

        // Act
        var result = await handler.ExecuteQueryAsync(
            new GetBotsByOwnerUserIdQuery(TestOwnerUserId),
            CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ExecuteQueryAsync_ReturnsEmptyWhenNoBots()
    {
        // Arrange
        var store = CreateStoreMock();
        var logger = new Mock<ILogger<MyTelegram.QueryHandlers.MongoDB.Bot.GetBotsByOwnerUserIdQueryHandler>>();
        var handler = new MyTelegram.QueryHandlers.MongoDB.Bot.GetBotsByOwnerUserIdQueryHandler(store.Object, logger.Object);

        // Act
        var result = await handler.ExecuteQueryAsync(
            new GetBotsByOwnerUserIdQuery(999),
            CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }
}
