namespace MyTelegram.Domain.Sagas;

public class EditExportedChatInviteSagaState : AggregateState<EditExportedChatInviteSaga, EditExportedChatInviteSagaId, EditExportedChatInviteSagaState>,
    IApply<EditExportedChatInviteStartedSagaEvent>,
    IApply<EditExportedChatInviteCompletedSagaEvent>

{
    public ChatInviteEditedEvent ChatInviteEditedEvent { get; private set; } = null!;
    public void Apply(EditExportedChatInviteStartedSagaEvent aggregateEvent)
    {
        ChatInviteEditedEvent = aggregateEvent.ChatInviteEditedEvent;
    }

    public void Apply(EditExportedChatInviteCompletedSagaEvent aggregateEvent)
    {

    }
}

public class EditExportedChatInviteStartedSagaEvent(ChatInviteEditedEvent chatInviteEditedEvent) : AggregateEvent<EditExportedChatInviteSaga, EditExportedChatInviteSagaId>
{
    public ChatInviteEditedEvent ChatInviteEditedEvent { get; } = chatInviteEditedEvent;
}

public class EditExportedChatInviteCompletedSagaEvent(ChatInviteEditedEvent chatInviteEditedEvent) : AggregateEvent<EditExportedChatInviteSaga, EditExportedChatInviteSagaId>
{
    public ChatInviteEditedEvent ChatInviteEditedEvent { get; } = chatInviteEditedEvent;
}

public class EditExportedChatInviteSaga : MyInMemoryAggregateSaga<EditExportedChatInviteSaga, EditExportedChatInviteSagaId,
            EditExportedChatInviteSagaLocator>,
        ISagaIsStartedBy<ChatInviteAggregate, ChatInviteId, ChatInviteEditedEvent>,
        ISagaHandles<ChatInviteAggregate, ChatInviteId, ChatInviteExportedEvent>
{
    private readonly IIdGenerator _idGenerator;
    private readonly EditExportedChatInviteSagaState _state = new();
    public EditExportedChatInviteSaga(EditExportedChatInviteSagaId id,
        IEventStore eventStore,
        IIdGenerator idGenerator) : base(id, eventStore)
    {
        _idGenerator = idGenerator;
        Register(_state);
    }

    public async Task HandleAsync(IDomainEvent<ChatInviteAggregate, ChatInviteId, ChatInviteEditedEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        if (domainEvent.AggregateEvent.Revoked && domainEvent.AggregateEvent.Permanent)
        {
            Emit(new EditExportedChatInviteStartedSagaEvent(domainEvent.AggregateEvent));

            var inviteId = await _idGenerator.NextLongIdAsync(IdType.InviteId, domainEvent.AggregateEvent.ChannelId, cancellationToken: cancellationToken);
            var command = new ExportChatInviteCommand(
                ChatInviteId.Create(domainEvent.AggregateEvent.ChannelId, inviteId),
                domainEvent.AggregateEvent.RequestInfo,
                domainEvent.AggregateEvent.ChannelId,
                inviteId,
                domainEvent.AggregateEvent.NewHash!,
                domainEvent.AggregateEvent.AdminId,
                domainEvent.AggregateEvent.Title,
                domainEvent.AggregateEvent.RequestNeeded,
                domainEvent.AggregateEvent.StartDate,
                domainEvent.AggregateEvent.ExpireDate,
                domainEvent.AggregateEvent.UsageLimit,
                domainEvent.AggregateEvent.Permanent,
                DateTime.UtcNow.ToTimestamp(),
                domainEvent.AggregateEvent.IsBroadcast
            );
            Publish(command);
        }
        else
        {
            await HandleCompletedAsync(domainEvent.AggregateEvent);
        }
    }

    public Task HandleAsync(IDomainEvent<ChatInviteAggregate, ChatInviteId, ChatInviteExportedEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        return HandleCompletedAsync(_state.ChatInviteEditedEvent);
    }

    private Task HandleCompletedAsync(ChatInviteEditedEvent chatInviteEditedEvent)
    {
        Emit(new EditExportedChatInviteCompletedSagaEvent(chatInviteEditedEvent));
        return CompleteAsync();
    }
}
