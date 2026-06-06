using Moq;
using EventFlow.MongoDB.ReadStores;
using MongoDB.Driver;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.TestBase;
using Shouldly;
using Xunit;

namespace MyTelegram.Domain.Tests.UnitTests.QueryHandlers;

public class GetChatMemberListQueryHandlerTests : MyTelegramTestBase
{
    private static Mock<IAsyncCursor<ChatReadModel>> CreateCursorMock(List<ChatReadModel> items)
    {
        var cursorMock = new Mock<IAsyncCursor<ChatReadModel>>();
        var returned = false;
        cursorMock
            .Setup(c => c.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                if (returned) return false;
                returned = true;
                return true;
            });
        cursorMock
            .Setup(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                if (returned) return false;
                returned = true;
                return true;
            });
        cursorMock.Setup(c => c.Current).Returns(items);
        return cursorMock;
    }

    [Fact]
    public async Task ExecuteQueryAsync_ReturnsMemberUserIds()
    {
        // Arrange
        var chat = new ChatReadModel();
        typeof(ChatReadModel).GetProperty("ChatId")!.SetValue(chat, 42L);
        typeof(ChatReadModel).GetProperty("ChatMembers")!.SetValue(chat, new List<MyTelegram.ReadModel.Impl.ChatMember>
        {
            new() { UserId = 100, InviterId = 1, Date = 1700000000 },
            new() { UserId = 200, InviterId = 1, Date = 1700000000 },
            new() { UserId = 300, InviterId = 1, Date = 1700000000 }
        });

        var cursorMock = CreateCursorMock(new List<ChatReadModel> { chat });
        var storeMock = new Mock<IMongoDbReadModelStore<ChatReadModel>>();
        storeMock
            .Setup(s => s.FindAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<ChatReadModel, bool>>>(),
                It.IsAny<FindOptions<ChatReadModel, ChatReadModel>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

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
        var cursorMock = CreateCursorMock(new List<ChatReadModel>());
        var storeMock = new Mock<IMongoDbReadModelStore<ChatReadModel>>();
        storeMock
            .Setup(s => s.FindAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<ChatReadModel, bool>>>(),
                It.IsAny<FindOptions<ChatReadModel, ChatReadModel>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

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
