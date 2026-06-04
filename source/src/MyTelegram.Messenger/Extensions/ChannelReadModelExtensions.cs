namespace MyTelegram.Messenger.Extensions;

public static class ChannelReadModelExtensions
{
    public static void ThrowExceptionIfChannelDeleted(this IChannelReadModel? channelReadModel)
    {
        if (channelReadModel == null)
        {
            RpcErrors.RpcErrors400.ChannelInvalid.ThrowRpcError();
        }

        if (channelReadModel!.IsDeleted)
        {
            RpcErrors.RpcErrors400.ChannelPrivate.ThrowRpcError();
        }
    }
}