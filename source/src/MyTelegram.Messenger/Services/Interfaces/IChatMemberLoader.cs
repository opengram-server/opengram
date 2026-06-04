namespace MyTelegram.Messenger.Services.Interfaces;

public interface IChatMemberLoader
{
    Task<List<long>> GetChatMemberUidListAsync(long chatId);
    Task<List<long>> GetChannelMemberUidListAsync(long channelId);
}
