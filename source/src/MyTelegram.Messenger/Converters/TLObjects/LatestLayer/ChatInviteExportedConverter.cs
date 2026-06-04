using IExportedChatInvite = MyTelegram.Schema.IExportedChatInvite;

namespace MyTelegram.Messenger.Converters.TLObjects.LatestLayer;

internal sealed class ChatInviteExportedConverter(IObjectMapper objectMapper) : IChatInviteExportedConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;
    public ILayeredExportedChatInvite ToExportedChatInvite(IChatInviteReadModel readModel)
    {
        return objectMapper.Map<IChatInviteReadModel, TChatInviteExported>(readModel);
    }

    public ILayeredExportedChatInvite ToExportedChatInvite(ChatInviteCreatedEvent source)
    {
        return objectMapper.Map<ChatInviteCreatedEvent, TChatInviteExported>(source);
    }

    public ILayeredExportedChatInvite ToExportedChatInvite(ChatInviteEditedEvent source)
    {
        return objectMapper.Map<ChatInviteEditedEvent, TChatInviteExported>(source);
    }

    public int RequestLayer { get; set; }
}
