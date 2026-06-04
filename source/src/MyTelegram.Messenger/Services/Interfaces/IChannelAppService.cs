namespace MyTelegram.Messenger.Services.Interfaces;

public interface IChannelAppService : IReadModelWithCacheAppService<IChannelReadModel>
{
    Task<IChannelFullReadModel?> GetChannelFullAsync(long channelId);
    Task<bool> IsChannelMemberAsync(long userId, long channelId);
    Task<bool> SendRpcErrorIfNotChannelMemberAsync(IRequestInput input, IChannelReadModel channelReadModel);
    Task<bool> SendRpcErrorIfNotChannelMemberAsync(IRequestInput input, long channelId);
}