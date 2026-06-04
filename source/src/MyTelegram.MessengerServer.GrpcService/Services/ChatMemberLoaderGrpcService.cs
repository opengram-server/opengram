using EventFlow.Queries;
using Grpc.Core;
using MyTelegram.GrpcService;
using MyTelegram.Queries;

namespace MyTelegram.MessengerServer.GrpcService.Services;

public class ChatMemberLoaderGrpcService(
    IQueryProcessor queryProcessor,
    ILogger<ChatMemberLoaderGrpcService> logger)
    : ChatMemberLoaderService.ChatMemberLoaderServiceBase
{
    public override async Task<GetChannelMemberResponse> GetChannelMembers(GetChannelMemberRequest request,
        ServerCallContext context)
    {
        var channelId = request.ChannelId;
        var channelMemberList = await queryProcessor
                .ProcessAsync(new GetChannelMembersByChannelIdQuery(channelId,
                        new List<long>(),
                        false,
                        0,
                        int.MaxValue),
                    default)
            ;
        var newMemberUidList = channelMemberList.Select(p => p.UserId).ToList();
        logger.LogDebug("Load channel member list from read model,channelId={ChannelId},member count={MemberCount}",
            channelId,
            newMemberUidList.Count);
        var r = new GetChannelMemberResponse();
        r.ChannelMemberUidList.AddRange(newMemberUidList);
        return r;
    }

    public override async Task<GetChatMemberResponse> GetChatMembers(GetChatMemberRequest request,
        ServerCallContext context)
    {
        var chatId = request.ChatId;
        var chatReadModel = await queryProcessor.ProcessAsync(new GetChatByChatIdQuery(chatId), default)
            ;
        if (chatReadModel == null)
        {
            logger.LogWarning("Get chat read model failed,chat read model not exists,chatId={ChatId}", chatId);
            return new GetChatMemberResponse();
        }

        var newMemberUidList = chatReadModel.ChatMembers.Select(p => p.UserId).ToList();
        //_chatToChatMembers.TryAdd(chatId, newMemberUidList);

        logger.LogDebug("Load chat member list from read model,chatId={ChatId},member count={MemberCount}",
            chatId,
            newMemberUidList.Count);
        var r = new GetChatMemberResponse();
        r.ChatMemberUidList.AddRange(newMemberUidList);
        return r;
    }
}
