using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.Commands.Story;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// See <a href="https://corefork.telegram.org/method/stories.deleteStories" />
///</summary>
internal sealed class DeleteStoriesHandler(
    ILogger<DeleteStoriesHandler> logger,
    ICommandBus commandBus) 
    : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestDeleteStories, TVector<int>>,
    Stories.IDeleteStoriesHandler
{
    protected override async Task<TVector<int>> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestDeleteStories obj)
    {
        logger.LogInformation("DeleteStories: UserId={UserId}, StoryIds={StoryIds}",
            input.UserId, string.Join(",", obj.Id));

        var deletedIds = new List<int>();

        // Удаляем каждую историю по очереди
        foreach (var storyId in obj.Id)
        {
            try
            {
                var command = new DeleteStoryCommand(
                    StoryId.Create(input.UserId, storyId));

                await commandBus.PublishAsync(command, CancellationToken.None);
                
                deletedIds.Add(storyId);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Failed to delete story {StoryId}: {Error}", storyId, ex.Message);
            }
        }

        logger.LogInformation("Stories deleted: Count={Count}", deletedIds.Count);

        return new TVector<int>(deletedIds);
    }
}
