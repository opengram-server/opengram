using MyTelegram.Schema.Messages;
using MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Messages;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

internal sealed class GetQuickRepliesHandler : RpcResultObjectHandler<RequestGetQuickReplies, IQuickReplies>, IGetQuickRepliesHandler
{
    protected override Task<IQuickReplies> HandleCoreAsync(IRequestInput input, RequestGetQuickReplies obj)
    {
        return Task.FromResult<IQuickReplies>(new TQuickReplies
        {
            QuickReplies = [],
            Messages = [],
            Chats = [],
            Users = []
        });
    }
}
