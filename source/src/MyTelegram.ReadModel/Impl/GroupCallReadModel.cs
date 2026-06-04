using EventFlow.MongoDB.ReadStores;
using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Events.GroupCall;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.ReadModel.Impl;

public class GroupCallReadModel : IGroupCallReadModel, IMongoDbReadModel,
    IAmReadModelFor<GroupCallAggregate, GroupCallId, GroupCallCreatedEvent>,
    IAmReadModelFor<GroupCallAggregate, GroupCallId, GroupCallParticipantJoinedEvent>,
    IAmReadModelFor<GroupCallAggregate, GroupCallId, GroupCallParticipantLeftEvent>,
    IAmReadModelFor<GroupCallAggregate, GroupCallId, GroupCallDiscardedEvent>,
    IAmReadModelFor<GroupCallAggregate, GroupCallId, GroupCallSettingsUpdatedEvent>,
    IAmReadModelFor<GroupCallAggregate, GroupCallId, GroupCallParticipantUpdatedEvent>,
    IAmReadModelFor<GroupCallAggregate, GroupCallId, GroupCallStartedEvent>,
    IAmReadModelFor<GroupCallAggregate, GroupCallId, GroupCallRecordToggledEvent>,
    IAmReadModelFor<GroupCallAggregate, GroupCallId, GroupCallTitleEditedEvent>
{
    public string Id { get; set; } = null!;
    public long CallId { get; private set; }
    public long AccessHash { get; private set; }
    public long PeerId { get; private set; }
    public string PeerType { get; private set; } = default!;
    public string? Title { get; private set; }
    public int ParticipantsCount { get; private set; }
    public bool JoinMuted { get; private set; }
    public bool CanChangeJoinMuted { get; private set; }
    public bool CanStartVideo { get; private set; }
    public bool RecordVideoActive { get; private set; }
    public bool RtmpStream { get; private set; }
    public int? StreamDcId { get; private set; }
    public int? RecordStartDate { get; private set; }
    public int? ScheduleDate { get; private set; }
    public int UnmutedVideoCount { get; private set; }
    public int UnmutedVideoLimit { get; private set; }
    public long? Version { get; set; }
    public int CreatedDate { get; private set; }
    public int? EndedDate { get; private set; }
    public bool IsDiscarded { get; private set; }
    public List<GroupCallParticipantReadModel> Participants { get; private set; } = new();

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<GroupCallAggregate, GroupCallId, GroupCallCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Id = domainEvent.AggregateIdentity.Value;
        CallId = domainEvent.AggregateEvent.CallId;
        AccessHash = domainEvent.AggregateEvent.AccessHash;
        PeerId = domainEvent.AggregateEvent.PeerId;
        PeerType = domainEvent.AggregateEvent.PeerType.ToString();
        Title = domainEvent.AggregateEvent.Title;
        ParticipantsCount = 0;
        JoinMuted = domainEvent.AggregateEvent.JoinMuted;
        CanChangeJoinMuted = true;
        CanStartVideo = true;
        RecordVideoActive = false;
        RtmpStream = domainEvent.AggregateEvent.RtmpStream;
        StreamDcId = domainEvent.AggregateEvent.StreamDcId;
        ScheduleDate = domainEvent.AggregateEvent.ScheduleDate;
        UnmutedVideoCount = 0;
        UnmutedVideoLimit = 30;
        Version = 1; // Initialize to 1 for MongoDB optimistic concurrency
        CreatedDate = domainEvent.AggregateEvent.Date;
        IsDiscarded = false;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<GroupCallAggregate, GroupCallId, GroupCallParticipantJoinedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var participant = new GroupCallParticipantReadModel
        {
            PeerId = domainEvent.AggregateEvent.PeerId,
            PeerType = domainEvent.AggregateEvent.PeerType.ToString(),
            Source = domainEvent.AggregateEvent.Source,
            JoinedDate = domainEvent.AggregateEvent.Date,
            Muted = domainEvent.AggregateEvent.Muted,
            CanSelfUnmute = true,
            Volume = 10000,
            VideoStopped = domainEvent.AggregateEvent.VideoStopped,
            Params = domainEvent.AggregateEvent.Params
        };

        Participants.Add(participant);
        ParticipantsCount++;
        Version++;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<GroupCallAggregate, GroupCallId, GroupCallParticipantLeftEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var participant = Participants.FirstOrDefault(p => p.PeerId == domainEvent.AggregateEvent.PeerId);
        if (participant != null)
        {
            participant.Left = true;
            participant.LeftDate = domainEvent.AggregateEvent.Date;
            ParticipantsCount--;
            Version++;
        }

        return Task.CompletedTask;
    }

    public int? Duration { get; private set; }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<GroupCallAggregate, GroupCallId, GroupCallDiscardedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        IsDiscarded = true;
        EndedDate = domainEvent.AggregateEvent.Date;
        Duration = EndedDate - CreatedDate;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<GroupCallAggregate, GroupCallId, GroupCallRecordToggledEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        RecordStartDate = domainEvent.AggregateEvent.Start ? domainEvent.AggregateEvent.Date : null;
        RecordVideoActive = domainEvent.AggregateEvent.Video;
        Version++;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<GroupCallAggregate, GroupCallId, GroupCallTitleEditedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Title = domainEvent.AggregateEvent.Title;
        Version++;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<GroupCallAggregate, GroupCallId, GroupCallSettingsUpdatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        if (domainEvent.AggregateEvent.JoinMuted.HasValue)
        {
            JoinMuted = domainEvent.AggregateEvent.JoinMuted.Value;
        }
        Version++;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<GroupCallAggregate, GroupCallId, GroupCallParticipantUpdatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var participant = Participants.FirstOrDefault(p => p.PeerId == domainEvent.AggregateEvent.PeerId);
        if (participant != null)
        {
            if (domainEvent.AggregateEvent.Muted.HasValue)
            {
                participant.Muted = domainEvent.AggregateEvent.Muted.Value;
                participant.CanSelfUnmute = !domainEvent.AggregateEvent.Muted.Value || !domainEvent.AggregateEvent.MutedByAdmin;
            }

            if (domainEvent.AggregateEvent.Volume.HasValue)
            {
                participant.Volume = domainEvent.AggregateEvent.Volume.Value;
            }

            if (domainEvent.AggregateEvent.RaiseHand.HasValue)
            {
                participant.RaiseHandRating = domainEvent.AggregateEvent.RaiseHand.Value
                    ? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    : null;
            }

            if (domainEvent.AggregateEvent.VideoStopped.HasValue)
            {
                participant.VideoStopped = domainEvent.AggregateEvent.VideoStopped.Value;
            }

            participant.ActiveDate = domainEvent.AggregateEvent.Date;
            Version++;
        }

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<GroupCallAggregate, GroupCallId, GroupCallStartedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        // Clear schedule date when call is started
        ScheduleDate = null;
        Version++;

        return Task.CompletedTask;
    }
}
