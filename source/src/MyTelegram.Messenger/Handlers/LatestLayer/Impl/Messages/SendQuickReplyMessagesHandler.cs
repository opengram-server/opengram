// ReSharper disable All

using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Messages;
using MyTelegram.Messenger.Extensions;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// See <a href="https://corefork.telegram.org/method/messages.sendQuickReplyMessages" />
///</summary>
internal sealed class SendQuickReplyMessagesHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSendQuickReplyMessages, MyTelegram.Schema.IUpdates>,
    ISendQuickReplyMessagesHandler
{
    private readonly IQuickRepliesAppService _quickRepliesAppService;

    public SendQuickReplyMessagesHandler(IQuickRepliesAppService quickRepliesAppService)
    {
        _quickRepliesAppService = quickRepliesAppService;
    }

    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestSendQuickReplyMessages obj)
    {
        await _quickRepliesAppService.SendQuickReplyMessagesAsync(input.UserId, obj.Peer.GetPeerId(), obj.ShortcutId);
        
        // TODO: Return actual updates
        return new TUpdates
        {
            Updates = new TVector<IUpdate>(),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = DateTime.UtcNow.ToTimestamp()
        };
    }
}
