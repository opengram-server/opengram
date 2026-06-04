using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Messages;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

public class ReorderQuickRepliesHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestReorderQuickReplies, IBool>, IReorderQuickRepliesHandler
{
    ///<summary>
/// Reorder <a href="https://corefork.telegram.org/api/business#quick-reply-shortcuts">quick reply shortcuts</a>.This will emit an <a href="https://corefork.telegram.org/constructor/updateQuickReplies">updateQuickReplies</a> update to other logged-in sessions.
/// <para>Possible errors</para>
/// Code Type Description
/// 403 PREMIUM_ACCOUNT_REQUIRED A premium account is required to execute this action.
/// See <a href="https://corefork.telegram.org/method/messages.reorderQuickReplies" />
///</summary>
    private readonly IQuickRepliesAppService _quickRepliesAppService;

    public ReorderQuickRepliesHandler(IQuickRepliesAppService quickRepliesAppService)
    {
        _quickRepliesAppService = quickRepliesAppService;
    }

    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestReorderQuickReplies obj)
    {
        await _quickRepliesAppService.ReorderQuickRepliesAsync(input.UserId, obj.Order.ToList());
        return new TBoolTrue();
    }
}
