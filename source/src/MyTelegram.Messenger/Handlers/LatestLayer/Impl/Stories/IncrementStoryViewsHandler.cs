using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.Commands.Story;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// See <a href="https://corefork.telegram.org/method/stories.incrementStoryViews" />
///</summary>
internal sealed class IncrementStoryViewsHandler(
    ILogger<IncrementStoryViewsHandler> logger,
    ICommandBus commandBus,
    IQueryProcessor queryProcessor) 
    : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestIncrementStoryViews, IBool>,
    Stories.IIncrementStoryViewsHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestIncrementStoryViews obj)
    {
        logger.LogInformation("IncrementStoryViews: UserId={UserId}, StoryIds={StoryIds}",
            input.UserId, string.Join(",", obj.Id));

        // Определяем идентификатор пира по переданному input peer
        var peerId = obj.Peer switch
        {
            TInputPeerSelf => input.UserId,
            TInputPeerUser user => user.UserId,
            TInputPeerChannel channel => channel.ChannelId,
            _ => 0L
        };

        if (peerId == 0)
        {
            throw new RpcException(RpcErrors.RpcErrors400.PeerIdInvalid);
        }

        // Увеличиваем счётчик просмотров для каждой истории
        foreach (var storyId in obj.Id)
        {
            // Загружаем текущую историю, чтобы увеличить число просмотров
            var story = await queryProcessor.ProcessAsync(
                new GetStoryByIdQuery(peerId, storyId),
                CancellationToken.None);

            if (story != null && !story.IsDeleted)
            {
                var command = new IncrementStoryViewsCommand(
                    StoryId.Create(peerId, storyId),
                    story.ViewsCount + 1);

                await commandBus.PublishAsync(command, CancellationToken.None);
            }
        }

        logger.LogInformation("Story views incremented: Count={Count}", obj.Id.Count);

        return new TBoolTrue();
    }
}
