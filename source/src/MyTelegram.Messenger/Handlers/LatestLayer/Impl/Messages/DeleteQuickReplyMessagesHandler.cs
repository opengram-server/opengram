// ReSharper disable All

using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Messages;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

public class DeleteQuickReplyMessagesHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestDeleteQuickReplyMessages, MyTelegram.Schema.IUpdates>, IDeleteQuickReplyMessagesHandler
{
    ///<summary>
/// See <a href="https://corefork.telegram.org/method/messages.deleteQuickReplyMessages" />
///</summary>
    private readonly IQuickRepliesAppService _quickRepliesAppService;

    public DeleteQuickReplyMessagesHandler(IQuickRepliesAppService quickRepliesAppService)
    {
        _quickRepliesAppService = quickRepliesAppService;
    }

    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestDeleteQuickReplyMessages obj)
    {
        await _quickRepliesAppService.DeleteQuickReplyMessagesAsync(input.UserId, obj.ShortcutId, obj.Id.ToList());
        
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
