namespace MyTelegram.Domain.Sagas.States;

public class CreateChannelSagaState : AggregateState<CreateChannelSaga, CreateChannelSagaId, CreateChannelSagaState>,
    IApply<CreateChannelSagaStartedSagaEvent>
{
    public RequestInfo RequestInfo { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public long RandomId { get; private set; }
    public bool MigratedFromChat { get; private set; }
    public bool AutoCreateFromChat { get; private set; }
    public bool Broadcast { get; private set; }
    public int? TtlPeriod { get; private set; }
    public bool IsTtlFromDefaultSetting { get; private set; }
    public long ChannelId { get; private set; }

    public List<long> MemberUserIds { get; private set; } = [];
    public List<long> BotUserIds { get; private set; } = [];

    public bool ShouldCreateChannelMember => MemberUserIds.Count > 0 || BotUserIds.Count > 0;

    public void Apply(CreateChannelSagaStartedSagaEvent aggregateEvent)
    {
        RequestInfo = aggregateEvent.RequestInfo;
        ChannelId = aggregateEvent.ChannelId;
        Title = aggregateEvent.Title;
        RandomId = aggregateEvent.RandomId;
        MigratedFromChat = aggregateEvent.MigratedFromChat;
        AutoCreateFromChat = aggregateEvent.AutoCreateFromChat;
        Broadcast = aggregateEvent.Broadcast;
        TtlPeriod = aggregateEvent.TtlPeriod;
        IsTtlFromDefaultSetting = aggregateEvent.IsTtlFromDefaultSetting;
        MemberUserIds = aggregateEvent.MemberUserIds;
        BotUserIds = aggregateEvent.BotUserIds;
    }
}