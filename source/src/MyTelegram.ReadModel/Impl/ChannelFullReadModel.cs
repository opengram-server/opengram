using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Events.GroupCall;

namespace MyTelegram.ReadModel.Impl;

public class ChannelFullReadModel : IChannelFullReadModel,
    IAmReadModelFor<ChannelAggregate, ChannelId, ChannelCreatedEvent>,
    IAmReadModelFor<ChannelAggregate, ChannelId, DiscussionGroupUpdatedEvent>,
    IAmReadModelFor<ChannelAggregate, ChannelId, ChannelAboutEditedEvent>,
    IAmReadModelFor<ChannelAggregate, ChannelId, SlowModeChangedEvent>,
    IAmReadModelFor<ChannelAggregate, ChannelId, PreHistoryHiddenChangedEvent>,
    IAmReadModelFor<ChannelAggregate, ChannelId, NewMsgIdPinnedEvent>,
    IAmReadModelFor<ChannelAggregate, ChannelId, ChannelUserNameChangedEvent>,
    IAmReadModelFor<ChannelMemberAggregate, ChannelMemberId, ChannelMemberBannedRightsChangedEvent>,
    IAmReadModelFor<ChannelMemberAggregate, ChannelMemberId, ChannelMemberCreatedEvent>,
    IAmReadModelFor<ChannelAggregate, ChannelId, ChannelAdminRightsEditedEvent>,
    IAmReadModelFor<ChannelAggregate, ChannelId, ChatJoinRequestHiddenEvent>,
    IAmReadModelFor<ChannelAggregate,ChannelId,ChatInviteRequestPendingUpdatedEvent>,
    IAmReadModelFor<ChannelAggregate,ChannelId, LinkedChannelChangedEvent>,
    IAmReadModelFor<ChannelAggregate, ChannelId, ChannelAvailableReactionsChangedEvent>,
    IAmReadModelFor<ChannelAggregate, ChannelId, ChannelPaidMessagesPriceUpdatedEvent>,
    IAmReadModelFor<GroupCallAggregate, GroupCallId, GroupCallCreatedEvent>,
    IAmReadModelFor<GroupCallAggregate, GroupCallId, GroupCallDiscardedEvent>
{
    public virtual string? About { get; private set; }
    public virtual int AdminsCount { get; private set; }
    public virtual int? AvailableMinId { get; private set; }
    public virtual int BannedCount { get; private set; }
    public virtual bool CanSetLocation { get; private set; }
    public virtual bool CanSetStickers { get; private set; }
    public virtual bool CanSetUserName { get; private set; }
    public virtual bool CanViewParticipants { get; private set; } //= true;
    public virtual bool CanViewStats { get; private set; }
    public virtual long ChannelId { get; private set; }
    public virtual int? FolderId { get; private set; }
    //= true;
    public virtual bool HiddenPreHistory { get; private set; }

    public virtual string Id { get; private set; } = null!;
    public virtual int KickedCount { get; private set; }
    public virtual long? LinkedChatId { get; private set; }
    public virtual long? MigratedFromChatId { get; private set; }
    public virtual int? MigratedFromMaxId { get; private set; }
    public virtual int OnlineCount { get; private set; }
    public virtual int? PinnedMsgId { get; private set; }
    public virtual List<int> PinnedMsgIdList { get; protected set; } = new();
    public virtual int ReadInboxMaxId { get; set; }
    public virtual int ReadOutboxMaxId { get; set; }
    public virtual int? SlowModeNextSendDate { get; private set; }
    public virtual int? SlowModeSeconds { get; private set; }
    public virtual int UnreadCount { get; set; }
    public virtual string? UserName { get; private set; }
    public ReactionType ReactionType { get; private set; }
    public bool AllowCustomReaction { get; private set; }
    public List<string>? AvailableReactions { get; private set; }
    public bool AntiSpam { get; private set; }
    public int? TtlPeriod { get; private set; }
    public virtual long? Version { get; set; }
    public int? RequestsPending { get; private set; }
    public List<long>? RecentRequesters { get; private set; }
    public bool ParticipantsHidden { get; private set; }
    
    // Paid messages
    public long? SendPaidMessagesStars { get; private set; }
    public bool PaidMessagesAvailable { get; private set; }
    
    // Group call
    public long? ActiveGroupCallId { get; private set; }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChannelAggregate, ChannelId, ChannelAboutEditedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        About = domainEvent.AggregateEvent.About;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChannelAggregate, ChannelId, ChannelCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Id = domainEvent.AggregateIdentity.Value;
        ChannelId = domainEvent.AggregateEvent.ChannelId;
        About = domainEvent.AggregateEvent.About;
        CanViewParticipants = true;
        TtlPeriod = domainEvent.AggregateEvent.TtlPeriod;
        MigratedFromChatId = domainEvent.AggregateEvent.MigratedFromChatId;
        MigratedFromMaxId = domainEvent.AggregateEvent.MigratedMaxId;
        AdminsCount = 1;
        // Enable all reactions by default for new channels
        ReactionType = ReactionType.ReactionAll;
        AllowCustomReaction = true;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChannelAggregate, ChannelId, ChannelUserNameChangedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        UserName = domainEvent.AggregateEvent.UserName;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChannelAggregate, ChannelId, NewMsgIdPinnedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        if (domainEvent.AggregateEvent.Pinned)
        {
            PinnedMsgId = domainEvent.AggregateEvent.PinnedMsgId;
            PinnedMsgIdList.Add(PinnedMsgId.Value);
        }
        else
        {
            PinnedMsgIdList.Remove(domainEvent.AggregateEvent.PinnedMsgId);
            PinnedMsgId = PinnedMsgIdList.LastOrDefault();
        }

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChannelAggregate, ChannelId, PreHistoryHiddenChangedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        HiddenPreHistory = domainEvent.AggregateEvent.Hidden;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChannelAggregate, ChannelId, DiscussionGroupUpdatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        LinkedChatId = domainEvent.AggregateEvent.GroupChannelId;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChannelAggregate, ChannelId, SlowModeChangedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        SlowModeSeconds = domainEvent.AggregateEvent.Seconds;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChannelMemberAggregate, ChannelMemberId, ChannelMemberBannedRightsChangedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        if (!domainEvent.AggregateEvent.RemovedFromBanned)
        {
            BannedCount--;
        }

        if (domainEvent.AggregateEvent.RemovedFromKicked)
        {
            KickedCount--;
        }

        if (domainEvent.AggregateEvent.BannedRights.ViewMessages)
        {
            KickedCount++;
        }
        else
        {
            if (domainEvent.AggregateEvent.Banned)
            {
                BannedCount++;
            }
        }

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChannelMemberAggregate, ChannelMemberId, ChannelMemberCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        if (domainEvent.AggregateEvent.IsRejoin && domainEvent.AggregateEvent.BannedRights != null)
        {
            if (domainEvent.AggregateEvent.BannedRights.ViewMessages)
            {
                KickedCount--;
            }
            else if (domainEvent.AggregateEvent.BannedRights.ToIntValue() !=
                       ChatBannedRights.CreateDefaultBannedRights().ToIntValue())
            {
                BannedCount--;
            }
        }

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<ChannelAggregate, ChannelId, ChannelAdminRightsEditedEvent> domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent.AggregateEvent.RemoveAdminFromList)
        {
            AdminsCount--;
        }

        if (domainEvent.AggregateEvent.IsNewAdmin)
        {
            AdminsCount++;
        }

        return Task.CompletedTask;
    }
    public Task ApplyAsync(IReadModelContext context, IDomainEvent<ChannelAggregate, ChannelId, ChatJoinRequestHiddenEvent> domainEvent, CancellationToken cancellationToken)
    {
        RequestsPending = domainEvent.AggregateEvent.RequestsPending;
        RecentRequesters = domainEvent.AggregateEvent.RecentRequesters;

        return Task.CompletedTask;
    }
    public Task ApplyAsync(IReadModelContext context, IDomainEvent<ChannelAggregate, ChannelId, ChatInviteRequestPendingUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        RequestsPending = domainEvent.AggregateEvent.RequestsPending;
        RecentRequesters = domainEvent.AggregateEvent.RecentRequesters;

        return Task.CompletedTask;
    }
    public Task ApplyAsync(IReadModelContext context, IDomainEvent<ChannelAggregate, ChannelId, LinkedChannelChangedEvent> domainEvent, CancellationToken cancellationToken)
    {
        LinkedChatId = domainEvent.AggregateEvent.LinkedChannelId;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<ChannelAggregate, ChannelId, ChannelAvailableReactionsChangedEvent> domainEvent, CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        ReactionType = evt.ReactionType;
        AllowCustomReaction = evt.AllowCustomReaction;
        AvailableReactions = evt.AvailableReactions;
        
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<ChannelAggregate, ChannelId, ChannelPaidMessagesPriceUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        SendPaidMessagesStars = domainEvent.AggregateEvent.SendPaidMessagesStars;
        // Enable paid messages feature when price is set
        PaidMessagesAvailable = domainEvent.AggregateEvent.SendPaidMessagesStars.HasValue && domainEvent.AggregateEvent.SendPaidMessagesStars.Value > 0;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<GroupCallAggregate, GroupCallId, GroupCallCreatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        // Only update if this is for our channel
        if (domainEvent.AggregateEvent.PeerType == PeerType.Channel && domainEvent.AggregateEvent.PeerId == ChannelId)
        {
            ActiveGroupCallId = domainEvent.AggregateEvent.CallId;
        }
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<GroupCallAggregate, GroupCallId, GroupCallDiscardedEvent> domainEvent, CancellationToken cancellationToken)
    {
        // Only update if this is for our channel and this is our active call
        if (domainEvent.AggregateEvent.PeerType == PeerType.Channel && 
            domainEvent.AggregateEvent.PeerId == ChannelId &&
            ActiveGroupCallId == domainEvent.AggregateEvent.CallId)
        {
            ActiveGroupCallId = null;
        }
        return Task.CompletedTask;
    }
}