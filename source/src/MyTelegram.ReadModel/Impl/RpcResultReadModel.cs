namespace MyTelegram.ReadModel.Impl;

public class RpcResultReadModel : IRpcResultReadModel,
    IAmReadModelFor<RpcResultAggregate, RpcResultId, RpcResultCreatedEvent>,
    IAmReadModelFor<RpcResultAggregate, RpcResultId, RpcResultDeletedEvent>
{
    public int Date { get; private set; }

    public virtual string Id { get; private set; } = null!;
    public virtual long ReqMsgId { get; private set; }
    public virtual byte[] RpcData { get; private set; } = null!;
    public virtual long UserId { get; private set; }
    public virtual long? Version { get; set; }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<RpcResultAggregate, RpcResultId, RpcResultCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Id = domainEvent.AggregateIdentity.Value;
        UserId = domainEvent.AggregateEvent.RequestInfo.UserId;
        ReqMsgId = domainEvent.AggregateEvent.RequestInfo.ReqMsgId;
        RpcData = domainEvent.AggregateEvent.RpcData;
        Date = domainEvent.AggregateEvent.Date;

        return Task.CompletedTask;
    }
    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<RpcResultAggregate, RpcResultId, RpcResultDeletedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        context.MarkForDeletion();

        return Task.CompletedTask;
    }
}