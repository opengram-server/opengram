using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.Commands.Story;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// See <a href="https://corefork.telegram.org/method/stories.sendStory" />
///</summary>
internal sealed class SendStoryHandler(
    ILogger<SendStoryHandler> logger,
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    IUserAppService userAppService) 
    : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestSendStory, MyTelegram.Schema.IUpdates>,
    Stories.ISendStoryHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestSendStory obj)
    {
        logger.LogDebug("SendStory: UserId={UserId}", input.UserId);

        // Проверяем, может ли пользователь публиковать истории (нужен premium)
        var userReadModel = await userAppService.GetAsync(input.UserId);
        if (userReadModel == null)
        {
            throw new RpcException(RpcErrors.RpcErrors400.UserIdInvalid);
        }

        if (!userReadModel.Premium && !userReadModel.Bot)
        {
            throw new RpcException(RpcErrors.RpcErrors400.PremiumAccountRequired);
        }

        // Генерируем идентификатор истории
        var storyId = (int)await idGenerator.NextIdAsync(IdType.MessageId, input.UserId);
        var date = CurrentDate;
        var expireDate = date + (obj.Period ?? 86400); // по умолчанию 24 часа

        // Извлекаем данные медиа (упрощённо; фото и видео нужно обрабатывать полноценно)
        byte[] mediaData = Array.Empty<byte>();
        if (obj.Media is TInputMediaUploadedPhoto uploadedPhoto)
        {
            // Медиа уже загружено, сохраняем ссылку на него
            mediaData = System.Text.Encoding.UTF8.GetBytes($"photo_{uploadedPhoto.File}");
        }
        else if (obj.Media is TInputMediaUploadedDocument uploadedDoc)
        {
            mediaData = System.Text.Encoding.UTF8.GetBytes($"video_{uploadedDoc.File}");
        }

        // Извлекаем правила приватности
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
                        TInputUser inputUser => inputUser.UserId,
                        _ => 0L
                    }).Where(id => id != 0));
                }
            }
        }

        // Формируем команду создания истории
        var command = new SendStoryCommand(
            StoryId.Create(input.UserId, storyId),
            input.ToRequestInfo(),
            input.UserId,
            storyId,
            mediaData,
            obj.Caption,
            privacyRules,
            date,
            expireDate,
            obj.Pinned,
            obj.Noforwards,
            !obj.PrivacyRules?.Any() ?? true);

        await commandBus.PublishAsync(command, CancellationToken.None);

        logger.LogDebug("Story created: StoryId={StoryId}, UserId={UserId}", storyId, input.UserId);

        // Возвращаем обновления клиенту
        return new TUpdates
        {
            Updates = new TVector<IUpdate>
            {
                new TUpdateStory
                {
                    Peer = new TPeerUser { UserId = input.UserId },
                    Story = new TStoryItem
                    {
                        Id = storyId,
                        Date = date,
                        ExpireDate = expireDate,
                        Caption = obj.Caption,
                        Pinned = obj.Pinned,
                        Noforwards = obj.Noforwards,
                        Views = new TStoryViews
                        {
                            ViewsCount = 0
                        }
                    }
                }
            },
            Chats = new TVector<IChat>(),
            Users = new TVector<IUser>(),
            Date = date,
            Seq = 0
        };
    }
}
