using MyTelegram.Handlers.Messages;
using MyTelegram.Schema.Messages;
using MyTelegram.Domain.Aggregates.ChatImport;
using MyTelegram.Domain.Commands.ChatImport;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

internal sealed class StartHistoryImportHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestStartHistoryImport, MyTelegram.Schema.IBool>,
    Messages.IStartHistoryImportHandler
{
    private readonly ICommandBus _commandBus;
    private readonly IPeerHelper _peerHelper;

    public StartHistoryImportHandler(ICommandBus commandBus, IPeerHelper peerHelper)
    {
        _commandBus = commandBus;
        _peerHelper = peerHelper;
    }

    protected override async Task<MyTelegram.Schema.IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestStartHistoryImport obj)
    {
        var peer = _peerHelper.GetPeer(obj.Peer, input.UserId);
        
        var command = new StartChatImportCommand(ChatImportId.Create(obj.ImportId), peer.PeerId, obj.ImportId);
        await _commandBus.PublishAsync(command, CancellationToken.None);

        return new TBoolTrue();
    }
}
