// ReSharper disable All

using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Messages;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

public class GetQuickReplyMessagesHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetQuickReplyMessages, MyTelegram.Schema.Messages.IMessages>, IGetQuickReplyMessagesHandler
{
    ///<summary>
/// See <a href="https://corefork.telegram.org/method/messages.getQuickReplyMessages" />
///</summary>
    private readonly IQuickRepliesAppService _quickRepliesAppService;

    public GetQuickReplyMessagesHandler(IQuickRepliesAppService quickRepliesAppService)
    {
        _quickRepliesAppService = quickRepliesAppService;
    }

    protected override async Task<MyTelegram.Schema.Messages.IMessages> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetQuickReplyMessages obj)
    {
        var messages = await _quickRepliesAppService.GetQuickReplyMessagesAsync(input.UserId, obj.ShortcutId, obj.Id?.ToList(), obj.Hash);
        return messages;
    }
}
