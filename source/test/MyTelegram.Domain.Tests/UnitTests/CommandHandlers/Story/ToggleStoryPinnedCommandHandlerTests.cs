using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.CommandHandlers.Story;
using MyTelegram.Domain.Commands.Story;
using MyTelegram.TestBase;
using Shouldly;
using Xunit;

namespace MyTelegram.Domain.Tests.UnitTests.CommandHandlers.Story;

public class ToggleStoryPinnedCommandHandlerTests : TestsFor<ToggleStoryPinnedCommandHandler>
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
    public async Task ExecuteAsync_Pin_EmitsPinnedEvent()
    {
        var aggregate = CreateStoryAggregate();
        var command = new ToggleStoryPinnedCommand(
            StoryId.Create(TestPeerId, TestStoryId),
            A<RequestInfo>(),
            true);

        await Sut.ExecuteAsync(aggregate, command, CancellationToken.None);

        var evt = aggregate.UncommittedEvents.Last().AggregateEvent
            .ShouldBeOfType<MyTelegram.Domain.Aggregates.Story.Events.StoryPinnedEvent>();
        evt.Pinned.ShouldBeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_Unpin_EmitsUnpinnedEvent()
    {
        var aggregate = CreateStoryAggregate();
        var command = new ToggleStoryPinnedCommand(
            StoryId.Create(TestPeerId, TestStoryId),
            A<RequestInfo>(),
            false);

        await Sut.ExecuteAsync(aggregate, command, CancellationToken.None);

        var evt = aggregate.UncommittedEvents.Last().AggregateEvent
            .ShouldBeOfType<MyTelegram.Domain.Aggregates.Story.Events.StoryPinnedEvent>();
        evt.Pinned.ShouldBeFalse();
    }
}
