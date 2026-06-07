using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.CommandHandlers.Story;
using MyTelegram.Domain.Commands.Story;
using MyTelegram.TestBase;
using Shouldly;
using Xunit;

namespace MyTelegram.Domain.Tests.UnitTests.CommandHandlers.Story;

public class AddStoryReactionCommandHandlerTests : TestsFor<AddStoryReactionCommandHandler>
{
    private const long TestPeerId = 12345;
    private const int TestStoryId = 1;

    private StoryAggregate CreateStoryAggregate()
    {
        var aggregate = new StoryAggregate(StoryId.Create(TestPeerId, TestStoryId));
        var media = System.Text.Encoding.UTF8.GetBytes("test_media");
        aggregate.CreateStory(TestPeerId, TestStoryId, media, "caption",
            null, 1700000000, 1700086400, false, false, true);
        return aggregate;
    }

    [Fact]
    public async Task ExecuteAsync_WithEmoji_AddsReaction()
    {
        var aggregate = CreateStoryAggregate();
        var command = new AddStoryReactionCommand(
            StoryId.Create(TestPeerId, TestStoryId),
            A<RequestInfo>(),
            99999,
            "👍");

        await Sut.ExecuteAsync(aggregate, command, CancellationToken.None);

        var evt = aggregate.UncommittedEvents.Last().AggregateEvent
            .ShouldBeOfType<MyTelegram.Domain.Aggregates.Story.Events.StoryReactionAddedEvent>();
        evt.UserId.ShouldBe(99999);
        evt.Reaction.ShouldBe("👍");
    }

    [Fact]
    public async Task ExecuteAsync_WithCustomEmoji_AddsReaction()
    {
        var aggregate = CreateStoryAggregate();
        var command = new AddStoryReactionCommand(
            StoryId.Create(TestPeerId, TestStoryId),
            A<RequestInfo>(),
            88888,
            "custom_5678");

        await Sut.ExecuteAsync(aggregate, command, CancellationToken.None);

        var evt = aggregate.UncommittedEvents.Last().AggregateEvent
            .ShouldBeOfType<MyTelegram.Domain.Aggregates.Story.Events.StoryReactionAddedEvent>();
        evt.UserId.ShouldBe(88888);
        evt.Reaction.ShouldBe("custom_5678");
    }
}
