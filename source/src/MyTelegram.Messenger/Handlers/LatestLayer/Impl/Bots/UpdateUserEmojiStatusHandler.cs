namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Update the emoji status of a user (called by bots).
/// See <a href="https://corefork.telegram.org/method/bots.updateUserEmojiStatus" />
///</summary>
internal sealed class UpdateUserEmojiStatusHandler(
    ICommandBus commandBus,
    IUserAppService userAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestUpdateUserEmojiStatus, IBool>,
        Bots.IUpdateUserEmojiStatusHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestUpdateUserEmojiStatus obj)
    {
        // Verify the caller is a bot
        var botUser = await userAppService.GetAsync(input.UserId);
        if (botUser == null || !botUser.Bot)
        {
            throw new RpcException(new RpcError(400, "BOT_INVALID"));
        }

        // Target user
        var targetUserId = obj.UserId switch
        {
            TInputUser u => u.UserId,
            _ => throw new RpcException(RpcErrors.RpcErrors400.UserIdInvalid)
        };

        long? emojiDocumentId = null;
        int? emojiValidUntil = null;

        if (obj.EmojiStatus != null)
        {
            switch (obj.EmojiStatus)
            {
                case TEmojiStatus emojiStatus:
                    emojiDocumentId = emojiStatus.DocumentId;
                    emojiValidUntil = emojiStatus.Until;
                    break;
                case TEmojiStatusCollectible collectible:
                    emojiDocumentId = collectible.DocumentId;
                    emojiValidUntil = collectible.Until;
                    break;
                case TEmojiStatusEmpty:
                    // Clear emoji status
                    break;
            }
        }

        var command = new MyTelegram.Domain.Commands.User.UpdateUserEmojiStatusCommand(
            UserId.Create(targetUserId),
            input.ToRequestInfo(),
            emojiDocumentId,
            emojiValidUntil);

        await commandBus.PublishAsync(command, CancellationToken.None);

        return new TBoolTrue();
    }
}
