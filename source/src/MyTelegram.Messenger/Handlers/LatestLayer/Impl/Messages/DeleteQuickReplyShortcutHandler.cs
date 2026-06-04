// ReSharper disable All

using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Messages;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

public class DeleteQuickReplyShortcutHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestDeleteQuickReplyShortcut, IBool>, IDeleteQuickReplyShortcutHandler
{
    ///<summary>
/// See <a href="https://corefork.telegram.org/method/messages.deleteQuickReplyShortcut" />
///</summary>
    private readonly IQuickRepliesAppService _quickRepliesAppService;

    public DeleteQuickReplyShortcutHandler(IQuickRepliesAppService quickRepliesAppService)
    {
        _quickRepliesAppService = quickRepliesAppService;
    }

    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestDeleteQuickReplyShortcut obj)
    {
        await _quickRepliesAppService.DeleteQuickReplyShortcutAsync(input.UserId, obj.ShortcutId);
        return new TBoolTrue();
    }
}
