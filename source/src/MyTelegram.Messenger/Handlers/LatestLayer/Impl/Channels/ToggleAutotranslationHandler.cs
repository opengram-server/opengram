using MyTelegram.Messenger.Handlers.Channels;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Channels;

///<summary>
/// See <a href="https://corefork.telegram.org/method/channels.toggleAutotranslation" />
///</summary>
internal sealed class ToggleAutotranslationHandler(IChannelAppService channelAppService, ICommandBus commandBus, IAccessHashHelper accessHashHelper) : RpcResultObjectHandler<MyTelegram.Schema.Channels.RequestToggleAutotranslation, MyTelegram.Schema.IUpdates>,
    IToggleAutotranslationHandler
{
    protected override Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Channels.RequestToggleAutotranslation obj)
    {
        return Task.FromResult<MyTelegram.Schema.IUpdates>(new TUpdates
        {
            Chats = [],
            Updates = [],
            Users = [],
            Date = CurrentDate
        });
    }
}
