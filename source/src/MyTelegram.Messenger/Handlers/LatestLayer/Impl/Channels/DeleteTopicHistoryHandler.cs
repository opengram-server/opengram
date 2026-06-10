namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Channels;

///<summary>
/// Delete message history of a forum topic.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHANNEL_INVALID The provided channel is invalid.
/// 400 TOPIC_ID_INVALID The specified topic ID is invalid.
/// See <a href="https://corefork.telegram.org/method/channels.deleteTopicHistory" />
///</summary>
internal sealed class DeleteTopicHistoryHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    IAccessHashHelper accessHashHelper,
    IChannelAdminRightsChecker channelAdminRightsChecker)
    : RpcResultObjectHandler<MyTelegram.Schema.Channels.RequestDeleteTopicHistory, MyTelegram.Schema.Messages.IAffectedHistory>,
        Channels.IDeleteTopicHistoryHandler
{
    protected override async Task<IAffectedHistory> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Channels.RequestDeleteTopicHistory obj)
    {
        if (obj.Channel is TInputChannel inputChannel)
        {
            await accessHashHelper.CheckAccessHashAsync(input, inputChannel.ChannelId, inputChannel.AccessHash, AccessHashType.Channel);
            await channelAdminRightsChecker.CheckAdminRightAsync(inputChannel.ChannelId, input.UserId,
                p => p.AdminRights.DeleteMessages, RpcErrors.RpcErrors403.ChatAdminRequired);

            var messageIds = (await queryProcessor
                .ProcessAsync(new GetMessageIdListByChannelIdQuery(inputChannel.ChannelId,
                    MyTelegramConsts.ClearHistoryDefaultPageSize))).ToList();

            if (messageIds.Count > 0)
            {
                var newTopMessageId = await queryProcessor.ProcessAsync(
                    new GetTopMessageIdQuery(inputChannel.ChannelId, messageIds));
                var command = new StartDeleteParticipantHistoryCommand(TempId.New,
                    input.ToRequestInfo(),
                    inputChannel.ChannelId,
                    messageIds,
                    newTopMessageId);
                await commandBus.PublishAsync(command);
            }

            return null!;
        }

        throw new RpcException(RpcErrors.RpcErrors400.ChannelInvalid);
    }
}
