using MyTelegram.Domain.Events.GroupCall;

namespace MyTelegram.Domain.Aggregates.GroupCall;

public class GroupCallState : AggregateState<GroupCallAggregate, GroupCallId, GroupCallState>,
    IApply<GroupCallCreatedEvent>,
    IApply<GroupCallParticipantJoinedEvent>,
    IApply<GroupCallParticipantLeftEvent>,
    IApply<GroupCallDiscardedEvent>,
    IApply<GroupCallSettingsUpdatedEvent>,
    IApply<GroupCallParticipantUpdatedEvent>,
    IApply<GroupCallStartedEvent>,
    IApply<GroupCallTitleEditedEvent>,
    IApply<GroupCallRecordToggledEvent>
{
    public long CallId { get; private set; }
    public long AccessHash { get; private set; }
    public long PeerId { get; private set; }
    public PeerType PeerType { get; private set; }
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
    public int Version { get; private set; }
    public int CreatedDate { get; private set; }
    public int? EndedDate { get; private set; }
    public bool IsDiscarded { get; private set; }
    
    // Participants stored as dictionary for fast lookup
    public Dictionary<long, GroupCallParticipantState> Participants { get; private set; } = new();

    public void Apply(GroupCallCreatedEvent aggregateEvent)
    {
        CallId = aggregateEvent.CallId;
        AccessHash = aggregateEvent.AccessHash;
        PeerId = aggregateEvent.PeerId;
        PeerType = aggregateEvent.PeerType;
        Title = aggregateEvent.Title;
        ParticipantsCount = 0;
        JoinMuted = aggregateEvent.JoinMuted;
        CanChangeJoinMuted = true; // Creator can change
        CanStartVideo = true;
        RecordVideoActive = false;
        RtmpStream = aggregateEvent.RtmpStream;
        StreamDcId = aggregateEvent.StreamDcId;
        ScheduleDate = aggregateEvent.ScheduleDate;
        UnmutedVideoCount = 0;
        UnmutedVideoLimit = 30; // Default limit
        Version = 1;
        CreatedDate = aggregateEvent.Date;
        IsDiscarded = false;
    }

    public void Apply(GroupCallParticipantJoinedEvent aggregateEvent)
    {
        var participant = new GroupCallParticipantState
        {
            PeerId = aggregateEvent.PeerId,
            PeerType = aggregateEvent.PeerType,
            Source = aggregateEvent.Source,
            JoinedDate = aggregateEvent.Date,
            Muted = aggregateEvent.Muted,
            CanSelfUnmute = true,
            Volume = 10000, // Default volume
            VideoStopped = aggregateEvent.VideoStopped,
            Params = aggregateEvent.Params
        };
        
        Participants[aggregateEvent.PeerId] = participant;
        ParticipantsCount++;
        Version++;
    }

    public void Apply(GroupCallParticipantLeftEvent aggregateEvent)
    {
        if (Participants.TryGetValue(aggregateEvent.PeerId, out var participant))
        {
            participant.Left = true;
            participant.LeftDate = aggregateEvent.Date;
            ParticipantsCount--;
            Version++;
        }
    }

    public void Apply(GroupCallDiscardedEvent aggregateEvent)
    {
        IsDiscarded = true;
        EndedDate = aggregateEvent.Date;
    }

    public void Apply(GroupCallSettingsUpdatedEvent aggregateEvent)
    {
        if (aggregateEvent.JoinMuted.HasValue)
        {
            JoinMuted = aggregateEvent.JoinMuted.Value;
        }
        Version++;
    }

    public void Apply(GroupCallParticipantUpdatedEvent aggregateEvent)
    {
        if (Participants.TryGetValue(aggregateEvent.PeerId, out var participant))
        {
            if (aggregateEvent.Muted.HasValue)
            {
                participant.Muted = aggregateEvent.Muted.Value;
                participant.CanSelfUnmute = !aggregateEvent.Muted.Value || !aggregateEvent.MutedByAdmin;
            }
            
            if (aggregateEvent.Volume.HasValue)
            {
                participant.Volume = aggregateEvent.Volume.Value;
            }
            
            if (aggregateEvent.RaiseHand.HasValue)
            {
                participant.RaiseHandRating = aggregateEvent.RaiseHand.Value 
                    ? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() 
                    : null;
            }
            
            if (aggregateEvent.VideoStopped.HasValue)
            {
                participant.VideoStopped = aggregateEvent.VideoStopped.Value;
            }
            
            participant.ActiveDate = aggregateEvent.Date;
            Version++;
        }
    }

    public void Apply(GroupCallStartedEvent aggregateEvent)
    {
        // Clear schedule date when call is started
        ScheduleDate = null;
        Version++;
    }

    public void Apply(GroupCallTitleEditedEvent aggregateEvent)
    {
        Title = aggregateEvent.Title;
        Version++;
    }

    public void Apply(GroupCallRecordToggledEvent aggregateEvent)
    {
        RecordStartDate = aggregateEvent.Start ? aggregateEvent.Date : null;
        RecordVideoActive = aggregateEvent.Video;
        Version++;
    }
}

public class GroupCallParticipantState
{
    public long PeerId { get; set; }
    public PeerType PeerType { get; set; }
    public int Source { get; set; } // SSRC
    public int JoinedDate { get; set; }
    public int? ActiveDate { get; set; }
    public int? LeftDate { get; set; }
    public bool Muted { get; set; }
    public bool CanSelfUnmute { get; set; }
    public int Volume { get; set; }
    public string? About { get; set; }
    public long? RaiseHandRating { get; set; }
    public bool? VideoStopped { get; set; }
    public bool Left { get; set; }
    public string? Params { get; set; } // WebRTC params as JSON
}
