using EventFlow.Queries;
using MyTelegram.Messenger.Services.Interfaces;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.Messenger.Services.Impl;

public class ChatMemberLoader : IChatMemberLoader, ITransientDependency
{
    private readonly IQueryProcessor _queryProcessor;

    public ChatMemberLoader(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    public async Task<List<long>> GetChatMemberUidListAsync(long chatId)
    {
        var chatReadModel = await _queryProcessor.ProcessAsync(new GetChatByChatIdQuery(chatId), default);
        if (chatReadModel == null)
        {
            return [];
        }
        
        // Cast to ChatReadModel to access ChatMembers property
        if (chatReadModel is ChatReadModel chat)
        {
            return chat.ChatMembers.Select(p => p.UserId).ToList();
        }
        
        return [];
    }

    public async Task<List<long>> GetChannelMemberUidListAsync(long channelId)
    {
        var channelMemberList = await _queryProcessor.ProcessAsync(
            new GetChannelMembersByChannelIdQuery(channelId, [], 0, int.MaxValue),
            default);
        
        return channelMemberList.Select(p => p.UserId).ToList();
    }
}
