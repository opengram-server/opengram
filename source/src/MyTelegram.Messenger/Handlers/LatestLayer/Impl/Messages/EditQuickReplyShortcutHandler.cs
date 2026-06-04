// ReSharper disable All

using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Messages;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

public class EditQuickReplyShortcutHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestEditQuickReplyShortcut, IBool>, IEditQuickReplyShortcutHandler
{
    ///<summary>
/// See <a href="https://corefork.telegram.org/method/messages.editQuickReplyShortcut" />
///</summary>
    private readonly IQuickRepliesAppService _quickRepliesAppService;

    public EditQuickReplyShortcutHandler(IQuickRepliesAppService quickRepliesAppService)
    {
        _quickRepliesAppService = quickRepliesAppService;
    }

    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestEditQuickReplyShortcut obj)
    {
        var exists = await _quickRepliesAppService.CheckQuickReplyShortcutAsync(input.UserId, obj.Shortcut);
        if (exists)
        {
            await _quickRepliesAppService.EditQuickReplyShortcutAsync(input.UserId, obj.ShortcutId, obj.Shortcut);
        }
        else
        {
            await _quickRepliesAppService.CreateQuickReplyShortcutAsync(input.UserId, obj.Shortcut, obj.ShortcutId);
        }

        return new TBoolTrue();
    }
}
