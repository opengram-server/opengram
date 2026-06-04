using MyTelegram.Handlers.Messages;
using MyTelegram.Schema.Messages;
using MyTelegram.Domain.Aggregates.ChatImport;
using MyTelegram.Domain.Commands.ChatImport;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

internal sealed class UploadImportedMediaHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestUploadImportedMedia, MyTelegram.Schema.IMessageMedia>,
    Messages.IUploadImportedMediaHandler
{
    private readonly ICommandBus _commandBus;
    private readonly IPeerHelper _peerHelper;

    public UploadImportedMediaHandler(ICommandBus commandBus, IPeerHelper peerHelper)
    {
        _commandBus = commandBus;
        _peerHelper = peerHelper;
    }

    protected override async Task<MyTelegram.Schema.IMessageMedia> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestUploadImportedMedia obj)
    {
        var peer = _peerHelper.GetPeer(obj.Peer, input.UserId);
        
        // Dispatch command to update aggregate state
        var command = new UploadChatImportMediaCommand(ChatImportId.Create(obj.ImportId), peer.PeerId, obj.ImportId, obj.FileName, new byte[0]);
        await _commandBus.PublishAsync(command, CancellationToken.None);

        return new TMessageMediaEmpty(); 
    }
}
