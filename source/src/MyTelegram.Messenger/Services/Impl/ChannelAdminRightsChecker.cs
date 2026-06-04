namespace MyTelegram.Messenger.Services.Impl;

public class ChannelAdminRightsChecker(IQueryProcessor queryProcessor) : IChannelAdminRightsChecker, ITransientDependency
{
    public async Task CheckAdminRightAsync(long channelId, long userId, Func<IChatAdminReadModel, bool> checkAdminRightsFunc, RpcError rpcError)
    {
        if (!await HasChatAdminRightAsync(channelId, userId, checkAdminRightsFunc))
        {
            rpcError.ThrowRpcError();
        }
    }

    public async Task<bool> HasChatAdminRightAsync(long channelId, long userId, Func<IChatAdminReadModel, bool> checkAdminRightsFunc)
    {
        var chatAdmin = await queryProcessor.ProcessAsync(new GetChatAdminQuery(channelId, userId));
        if (chatAdmin == null)
        {
            return false;
        }

        return chatAdmin.IsCreator || checkAdminRightsFunc(chatAdmin);
    }
}