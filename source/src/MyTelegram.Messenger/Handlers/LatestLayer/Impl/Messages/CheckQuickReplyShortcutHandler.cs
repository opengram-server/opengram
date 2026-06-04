// ReSharper disable All

using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Messages;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// See <a href="https://corefork.telegram.org/method/messages.checkQuickReplyShortcut" />
///</summary>
internal sealed class CheckQuickReplyShortcutHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestCheckQuickReplyShortcut, IBool>,
    ICheckQuickReplyShortcutHandler
{
    private readonly IQuickRepliesAppService _quickRepliesAppService;

    public CheckQuickReplyShortcutHandler(IQuickRepliesAppService quickRepliesAppService)
    {
        _quickRepliesAppService = quickRepliesAppService;
    }

    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestCheckQuickReplyShortcut obj)
    {
        var exists = await _quickRepliesAppService.CheckQuickReplyShortcutAsync(input.UserId, obj.Shortcut);
        return exists ? new TBoolTrue() : new TBoolFalse();
    }
}
