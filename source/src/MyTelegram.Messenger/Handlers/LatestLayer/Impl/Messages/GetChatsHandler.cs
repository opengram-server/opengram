namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Returns chat basic info on their IDs.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHAT_ID_INVALID The provided chat id is invalid.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// See <a href="https://corefork.telegram.org/method/messages.getChats" />
///</summary>
internal sealed class GetChatsHandler(
    IQueryProcessor queryProcessor,
    IPeerHelper peerHelper,
    IChatConverterService chatConverterService)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetChats, MyTelegram.Schema.Messages.IChats>,
        Messages.IGetChatsHandler
{
    protected override async Task<MyTelegram.Schema.Messages.IChats> HandleCoreAsync(IRequestInput input,
        RequestGetChats obj)
    {
        var channelIds = new List<long>();

        foreach (var chatId in obj.Id)
        {
            channelIds.Add(chatId);
        }

        var channelMemberReadModels = await queryProcessor.ProcessAsync(
            new GetChannelMemberListByChannelIdListQuery(input.UserId, channelIds));
        var channels = await chatConverterService.GetChannelListAsync(input, channelIds, channelMemberReadModels, layer: input.Layer);

        return new MyTelegram.Schema.Messages.TChats
        {
            Chats = [.. channels]
        };
    }
}
