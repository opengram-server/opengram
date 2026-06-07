using Moq;
using EventFlow.MongoDB.ReadStores;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.TestBase;
using Shouldly;
using Xunit;

namespace MyTelegram.Domain.Tests.UnitTests.QueryHandlers;

public class GetChatMemberListQueryHandlerTests : MyTelegramTestBase
{
    [Fact]
    public async Task ExecuteQueryAsync_ReturnsMemberUserIds()
    {
        // Arrange
        var chat = new ChatReadModel();
        // Use reflection to set private setters
        typeof(ChatReadModel).GetProperty("ChatId")!.SetValue(chat, 42L);
        typeof(ChatReadModel).GetProperty("ChatMembers")!.SetValue(chat, new List<ChatMember>
        {
            new() { UserId = 100, InviterId = 1, Date = 1700000000 },
            new() { UserId = 200, InviterId = 1, Date = 1700000000 },
            new() { UserId = 300, InviterId = 1, Date = 1700000000 }
        });

        var storeMock = new Mock<IMongoDbReadModelStore<ChatReadModel>>();
        storeMock
            .Setup(s => s.FindAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<ChatReadModel, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ChatReadModel> { chat });

        var handler = new MyTelegram.QueryHandlers.MongoDB.Chat.GetChatMemberListQueryHandler(storeMock.Object);

        // Act
        var result = await handler.ExecuteQueryAsync(
            new GetChatMemberListQuery(42),
            CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
        result.ShouldContain(100);
        result.ShouldContain(200);
        result.ShouldContain(300);
    }

    [Fact]
    public async Task ExecuteQueryAsync_ReturnsEmptyWhenChatNotFound()
    {
        // Arrange
        var storeMock = new Mock<IMongoDbReadModelStore<ChatReadModel>>();
        storeMock
            .Setup(s => s.FindAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<ChatReadModel, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ChatReadModel>());

        var handler = new MyTelegram.QueryHandlers.MongoDB.Chat.GetChatMemberListQueryHandler(storeMock.Object);

        // Act
        var result = await handler.ExecuteQueryAsync(
            new GetChatMemberListQuery(999),
            CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }
}
