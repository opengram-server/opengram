using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.CommandHandlers.Story;
using MyTelegram.Domain.Commands.Story;
using MyTelegram.TestBase;
using Shouldly;
using Xunit;

namespace MyTelegram.Domain.Tests.UnitTests.CommandHandlers.Story;

public class EditStoryCommandHandlerTests : TestsFor<EditStoryCommandHandler>
{
    private const long TestPeerId = 12345;
    private const int TestStoryId = 1;

    private StoryAggregate CreateStoryAggregate()
    {
        var aggregate = new StoryAggregate(StoryId.Create(TestPeerId, TestStoryId));
        var media = System.Text.Encoding.UTF8.GetBytes("test_media");
        aggregate.CreateStory(TestPeerId, TestStoryId, media, "Original caption",
            null, 1700000000, 1700086400, false, false, true);
        return aggregate;
    }

    [Fact]
    public async Task ExecuteAsync_WithCaption_EditsStory()
    {
        var aggregate = CreateStoryAggregate();
        var command = new EditStoryCommand(
            StoryId.Create(TestPeerId, TestStoryId),
            A<RequestInfo>(),
            null,
            "New caption",
            null);

        await Sut.ExecuteAsync(aggregate, command, CancellationToken.None);

        var evt = aggregate.UncommittedEvents.Last().AggregateEvent
            .ShouldBeOfType<MyTelegram.Domain.Aggregates.Story.Events.StoryEditedEvent>();
        evt.Caption.ShouldBe("New caption");
        evt.Media.ShouldBeNull();
    }

    [Fact]
    public async Task ExecuteAsync_WithMedia_EditsStory()
    {
        var aggregate = CreateStoryAggregate();
        var newMedia = System.Text.Encoding.UTF8.GetBytes("new_media_data");
        var command = new EditStoryCommand(
            StoryId.Create(TestPeerId, TestStoryId),
            A<RequestInfo>(),
            newMedia,
            null,
            null);

        await Sut.ExecuteAsync(aggregate, command, CancellationToken.None);

        var evt = aggregate.UncommittedEvents.Last().AggregateEvent
            .ShouldBeOfType<MyTelegram.Domain.Aggregates.Story.Events.StoryEditedEvent>();
        evt.Media.ShouldNotBeNull();
    }

    [Fact]
    public async Task ExecuteAsync_WithPrivacyRules_EditsStory()
    {
        var aggregate = CreateStoryAggregate();
        var privacyRules = new List<long> { 100, 200 };
        var command = new EditStoryCommand(
            StoryId.Create(TestPeerId, TestStoryId),
            A<RequestInfo>(),
            null,
            null,
            privacyRules);

        await Sut.ExecuteAsync(aggregate, command, CancellationToken.None);

        var evt = aggregate.UncommittedEvents.Last().AggregateEvent
            .ShouldBeOfType<MyTelegram.Domain.Aggregates.Story.Events.StoryEditedEvent>();
        evt.PrivacyRules.ShouldNotBeNull();
        evt.PrivacyRules!.Count.ShouldBe(2);
    }
}
