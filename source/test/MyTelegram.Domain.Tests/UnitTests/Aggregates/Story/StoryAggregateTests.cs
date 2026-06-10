using EventFlow.Aggregates;
using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.Aggregates.Story.Events;
using MyTelegram.TestBase;
using Shouldly;
using Xunit;

namespace MyTelegram.Domain.Tests.UnitTests.Aggregates.Story;

public class StoryAggregateTests : TestsFor<StoryAggregate>
{
    private const long TestPeerId = 12345;
    private const int TestStoryId = 1;

    public StoryAggregateTests()
    {
        Fixture.Customize<StoryId>(x => x.FromFactory(() => StoryId.Create(TestPeerId, TestStoryId)));
    }

    private StoryAggregate CreateStoryAggregate()
    {
        var media = System.Text.Encoding.UTF8.GetBytes("test_media");
        Sut.CreateStory(TestPeerId, TestStoryId, media, "Test caption",
            new List<long> { 1, 2, 3 }, 1700000000, 1700086400, false, false, true);
        return Sut;
    }

    [Fact]
    public void CreateStory_EmitsStoryCreatedEvent()
    {
        var media = System.Text.Encoding.UTF8.GetBytes("test_media");
        Sut.CreateStory(TestPeerId, TestStoryId, media, "Test caption",
            new List<long> { 1 }, 1700000000, 1700086400, true, false, true);

        var evt = Sut.UncommittedEvents.Single().AggregateEvent.ShouldBeOfType<StoryCreatedEvent>();
        evt.PeerId.ShouldBe(TestPeerId);
        evt.StoryId.ShouldBe(TestStoryId);
        evt.Caption.ShouldBe("Test caption");
        evt.Pinned.ShouldBeTrue();
        evt.IsPublic.ShouldBeTrue();
        evt.NoForwards.ShouldBeFalse();
    }

    [Fact]
    public void EditStory_EmitsStoryEditedEvent()
    {
        CreateStoryAggregate();

        var newMedia = System.Text.Encoding.UTF8.GetBytes("new_media");
        Sut.EditStory(newMedia, "Updated caption", null);

        var evt = Sut.UncommittedEvents.Last().AggregateEvent.ShouldBeOfType<StoryEditedEvent>();
        evt.PeerId.ShouldBe(TestPeerId);
        evt.StoryId.ShouldBe(TestStoryId);
        evt.Caption.ShouldBe("Updated caption");
        evt.Media.ShouldNotBeNull();
    }

    [Fact]
    public void EditStory_WithNullMedia_PreservesExistingMedia()
    {
        CreateStoryAggregate();

        Sut.EditStory(null, "Only caption changed", null);

        var evt = Sut.UncommittedEvents.Last().AggregateEvent.ShouldBeOfType<StoryEditedEvent>();
        evt.Media.ShouldBeNull();
        evt.Caption.ShouldBe("Only caption changed");
    }

    [Fact]
    public void IncrementViews_EmitsStoryViewIncrementedEvent()
    {
        CreateStoryAggregate();

        Sut.IncrementViews(5);

        var evt = Sut.UncommittedEvents.Last().AggregateEvent.ShouldBeOfType<StoryViewIncrementedEvent>();
        evt.PeerId.ShouldBe(TestPeerId);
        evt.StoryId.ShouldBe(TestStoryId);
        evt.ViewsCount.ShouldBe(5);
    }

    [Fact]
    public void AddReaction_EmitsStoryReactionAddedEvent()
    {
        CreateStoryAggregate();

        Sut.AddReaction(99999, "👍");

        var evt = Sut.UncommittedEvents.Last().AggregateEvent.ShouldBeOfType<StoryReactionAddedEvent>();
        evt.PeerId.ShouldBe(TestPeerId);
        evt.StoryId.ShouldBe(TestStoryId);
        evt.UserId.ShouldBe(99999);
        evt.Reaction.ShouldBe("👍");
    }

    [Fact]
    public void AddReaction_WithCustomEmoji_EmitsEvent()
    {
        CreateStoryAggregate();

        Sut.AddReaction(88888, "custom_12345");

        var evt = Sut.UncommittedEvents.Last().AggregateEvent.ShouldBeOfType<StoryReactionAddedEvent>();
        evt.Reaction.ShouldBe("custom_12345");
    }

    [Fact]
    public void TogglePinned_EmitsStoryPinnedEvent()
    {
        CreateStoryAggregate();

        Sut.TogglePinned(true);

        var evt = Sut.UncommittedEvents.Last().AggregateEvent.ShouldBeOfType<StoryPinnedEvent>();
        evt.PeerId.ShouldBe(TestPeerId);
        evt.StoryId.ShouldBe(TestStoryId);
        evt.Pinned.ShouldBeTrue();
    }

    [Fact]
    public void TogglePinned_Unpin_EmitsCorrectEvent()
    {
        CreateStoryAggregate();

        Sut.TogglePinned(false);

        var evt = Sut.UncommittedEvents.Last().AggregateEvent.ShouldBeOfType<StoryPinnedEvent>();
        evt.Pinned.ShouldBeFalse();
    }

    [Fact]
    public void DeleteStory_EmitsStoryDeletedEvent()
    {
        CreateStoryAggregate();

        Sut.DeleteStory();

        var evt = Sut.UncommittedEvents.Last().AggregateEvent.ShouldBeOfType<StoryDeletedEvent>();
        evt.PeerId.ShouldBe(TestPeerId);
        evt.StoryId.ShouldBe(TestStoryId);
    }

    [Fact]
    public void OperationsOnUncreatedAggregate_ThrowsException()
    {
        // IncrementViews on uncreated aggregate should throw
        Should.Throw<Exception>(() => Sut.IncrementViews(1));
    }

    [Fact]
    public void MultipleEdits_EmitMultipleEvents()
    {
        CreateStoryAggregate();

        Sut.EditStory(null, "First edit", null);
        Sut.EditStory(null, "Second edit", null);

        var events = Sut.UncommittedEvents.ToList();
        events.Count.ShouldBe(3); // Created + 2 edits
        events[1].AggregateEvent.ShouldBeOfType<StoryEditedEvent>()
            .Caption.ShouldBe("First edit");
        events[2].AggregateEvent.ShouldBeOfType<StoryEditedEvent>()
            .Caption.ShouldBe("Second edit");
    }

    [Fact]
    public void CreateStory_WithPrivacyRules_EmitsCorrectEvent()
    {
        var media = System.Text.Encoding.UTF8.GetBytes("test_media");
        var privacyRules = new List<long> { 100, 200, 300 };

        Sut.CreateStory(TestPeerId, TestStoryId, media, null,
            privacyRules, 1700000000, 1700086400, false, true, false);

        var evt = Sut.UncommittedEvents.Single().AggregateEvent.ShouldBeOfType<StoryCreatedEvent>();
        evt.PrivacyRules.ShouldNotBeNull();
        evt.PrivacyRules!.Count.ShouldBe(3);
        evt.NoForwards.ShouldBeTrue();
        evt.IsPublic.ShouldBeFalse();
        evt.Caption.ShouldBeNull();
    }
}
