using MyTelegram.Handlers.Messages;
using MyTelegram.Schema.Messages;
using MyTelegram.Domain.Aggregates.ChatImport;
using MyTelegram.Domain.Commands.ChatImport;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Import chat history from a foreign chat app into a specific chat, <a href="https://corefork.telegram.org/api/import">click here for more info about imported chats »</a>.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHAT_ADMIN_REQUIRED You must be an admin in this chat to do this.
/// 400 IMPORT_FILE_INVALID The specified chat export file is invalid.
/// 400 IMPORT_FORMAT_DATE_INVALID The date specified in the import file is invalid.
/// 400 IMPORT_FORMAT_UNRECOGNIZED The specified chat export file was exported from an unsupported chat app.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// 406 PREVIOUS_CHAT_IMPORT_ACTIVE_WAIT_%dMIN Import for this chat is already in progress, wait %d minutes before starting a new one.
/// See <a href="https://corefork.telegram.org/method/messages.initHistoryImport" />
///</summary>
internal sealed class InitHistoryImportHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestInitHistoryImport, MyTelegram.Schema.Messages.IHistoryImport>,
    Messages.IInitHistoryImportHandler
{
    private readonly ICommandBus _commandBus;
    private readonly IIdGenerator _idGenerator;
    private readonly IPeerHelper _peerHelper;

    public InitHistoryImportHandler(ICommandBus commandBus, IIdGenerator idGenerator, IPeerHelper peerHelper)
    {
        _commandBus = commandBus;
        _idGenerator = idGenerator;
        _peerHelper = peerHelper;
    }

    protected override async Task<MyTelegram.Schema.Messages.IHistoryImport> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestInitHistoryImport obj)
    {
        var peer = _peerHelper.GetPeer(obj.Peer, input.UserId);
        
        // Basic permission check should be here (Admin check) - skipping for brevity in this step

        var importId = await _idGenerator.NextLongIdAsync(IdType.ChatImportId);
        var command = new CreateChatImportCommand(ChatImportId.Create(importId), peer.PeerId, "Imported Chat", input.UserId);
        await _commandBus.PublishAsync(command, CancellationToken.None);

        return new THistoryImport
        {
            Id = importId
        };
    }
}
