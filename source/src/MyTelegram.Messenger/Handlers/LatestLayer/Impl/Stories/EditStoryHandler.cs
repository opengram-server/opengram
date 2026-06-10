using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.Commands.Story;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// Edit an active story.
/// See <a href="https://corefork.telegram.org/method/stories.editStory" />
///</summary>
internal sealed class EditStoryHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor)
    : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestEditStory, MyTelegram.Schema.IUpdates>,
    Stories.IEditStoryHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestEditStory obj)
    {
        // Resolve the peer for the story
        long peerId = input.UserId;
        if (obj.Peer is TInputPeerSelf)
        {
            peerId = input.UserId;
        }
        else if (obj.Peer is TInputPeerUser inputPeerUser)
        {
            peerId = inputPeerUser.UserId;
        }
        else if (obj.Peer is TInputPeerChannel inputPeerChannel)
        {
            peerId = inputPeerChannel.ChannelId;
        }

        // Extract caption if provided
        string? caption = obj.Caption;

        // Extract privacy rules if provided
        List<long>? privacyRules = null;
        if (obj.PrivacyRules != null && obj.PrivacyRules.Count > 0)
        {
            privacyRules = new List<long>();
            foreach (var rule in obj.PrivacyRules)
            {
                if (rule is TInputPrivacyValueAllowUsers allowUsers)
                {
                    privacyRules.AddRange(allowUsers.Users.Select(u => u switch
                    {
                        TInputUser iu => iu.UserId,
                        _ => 0L
                    }).Where(id => id != 0));
                }
            }
        }

        // Extract media data if provided
        byte[]? mediaData = null;
        if (obj.Media != null)
        {
            if (obj.Media is TInputMediaUploadedPhoto uploadedPhoto)
            {
                mediaData = Encoding.UTF8.GetBytes($"photo_{uploadedPhoto.File}");
            }
            else if (obj.Media is TInputMediaUploadedDocument uploadedDoc)
            {
                mediaData = Encoding.UTF8.GetBytes($"video_{uploadedDoc.File}");
            }
        }

        var command = new EditStoryCommand(
            StoryId.Create(peerId, obj.Id),
            input.ToRequestInfo(),
            mediaData,
            caption,
            privacyRules);

        await commandBus.PublishAsync(command, CancellationToken.None);

        return null!;
    }
}
