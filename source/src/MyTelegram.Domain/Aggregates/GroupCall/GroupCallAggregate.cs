using MyTelegram.Domain.Shared;
using MyTelegram.Domain.Events.GroupCall;

namespace MyTelegram.Domain.Aggregates.GroupCall;

public class GroupCallAggregate : AggregateRoot<GroupCallAggregate, GroupCallId>
{
    private readonly GroupCallState _state = new();

    public GroupCallAggregate(GroupCallId id) : base(id)
    {
        Register(_state);
    }

    /// <summary>
    /// Gets the current state of the group call
    /// </summary>
    public GroupCallState State => _state;

    /// <summary>
    /// Creates a new group call
    /// </summary>
    public void CreateGroupCall(
        RequestInfo requestInfo,
        long callId,
        long accessHash,
        long peerId,
        PeerType peerType,
        string? title,
        bool rtmpStream,
        int? streamDcId,
        int? scheduleDate,
        int randomId,
        int date)
    {
        Specs.AggregateIsNew.ThrowDomainErrorIfNotSatisfied(this);

        Emit(new GroupCallCreatedEvent(
            requestInfo,
            callId,
            accessHash,
            peerId,
            peerType,
            title,
            rtmpStream,
            streamDcId,
            scheduleDate,
            randomId,
            date));
    }

    /// <summary>
    /// Participant joins the group call
    /// </summary>
    public void JoinGroupCall(
        RequestInfo requestInfo,
        long peerId,
        PeerType peerType,
        int source,
        bool muted,
        bool videoStopped,
        string? params_,
        int date)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

        if (_state.IsDiscarded)
            throw new InvalidOperationException("Cannot join a discarded group call");

        // Check for duplicate SSRC
        if (_state.Participants.Values.Any(p => p.Source == source && !p.Left && p.PeerId != peerId))
            throw new InvalidOperationException($"SSRC {source} is already in use");

        // Check if participant already joined
        if (_state.Participants.TryGetValue(peerId, out var existingParticipant) && !existingParticipant.Left)
        {
            // Participant is already in the call - this is OK for reconnections
            // Just return without emitting event (idempotent behavior)
            return;
        }

        Emit(new GroupCallParticipantJoinedEvent(
            requestInfo,
            _state.CallId,
            peerId,
            peerType,
            source,
            muted || _state.JoinMuted, // Apply join_muted setting
            videoStopped,
            params_,
            date));
    }

    /// <summary>
    /// Participant leaves the group call
    /// </summary>
    public void LeaveGroupCall(
        RequestInfo requestInfo,
        long peerId,
        int source,
        int date)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

        if (!_state.Participants.TryGetValue(peerId, out var participant))
            throw new InvalidOperationException("Participant not in the call");

        // Idempotent: if participant already left, do nothing
        if (participant.Left)
            return;

        if (participant.Source != source)
            throw new InvalidOperationException("Invalid source ID");

        Emit(new GroupCallParticipantLeftEvent(
            requestInfo,
            _state.CallId,
            peerId,
            source,
            date));
    }

    /// <summary>
    /// Discards/ends the group call
    /// </summary>
    public void DiscardGroupCall(
        RequestInfo requestInfo,
        int date)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

        if (_state.IsDiscarded)
            throw new InvalidOperationException("Group call already discarded");

        Emit(new GroupCallDiscardedEvent(
            requestInfo,
            _state.CallId,
            _state.PeerId,
            _state.PeerType,
            date));
    }

    /// <summary>
    /// Updates group call settings
    /// </summary>
    public void UpdateSettings(
        RequestInfo requestInfo,
        bool? joinMuted)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

        if (_state.IsDiscarded)
            throw new InvalidOperationException("Cannot update settings of discarded call");

        Emit(new GroupCallSettingsUpdatedEvent(
            requestInfo,
            _state.CallId,
            joinMuted));
    }

    /// <summary>
    /// Updates participant settings
    /// </summary>
    public void UpdateParticipant(
        RequestInfo requestInfo,
        long peerId,
        bool? muted,
        bool mutedByAdmin,
        int? volume,
        bool? raiseHand,
        bool? videoStopped,
        int date)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

        if (_state.IsDiscarded)
            throw new InvalidOperationException("Cannot update participant in discarded call");

        if (!_state.Participants.TryGetValue(peerId, out var participant))
            throw new InvalidOperationException("Participant not in the call");

        if (participant.Left)
            throw new InvalidOperationException("Participant already left");

        // Idempotent: check if anything actually changed
        var hasChanges = false;
        if (muted.HasValue && participant.Muted != muted.Value)
            hasChanges = true;
        if (mutedByAdmin && participant.CanSelfUnmute)
            hasChanges = true;
        if (volume.HasValue && participant.Volume != volume.Value)
            hasChanges = true;
        if (raiseHand.HasValue && participant.RaiseHandRating != (raiseHand.Value ? 1 : 0))
            hasChanges = true;
        if (videoStopped.HasValue && participant.VideoStopped != videoStopped.Value)
            hasChanges = true;

        // If nothing changed, don't emit event (idempotent)
        if (!hasChanges)
            return;

        Emit(new GroupCallParticipantUpdatedEvent(
            requestInfo,
            _state.CallId,
            peerId,
            muted,
            mutedByAdmin,
            volume,
            raiseHand,
            videoStopped,
            date));
    }

    /// <summary>
    /// Starts a scheduled group call
    /// </summary>
    public void StartScheduledGroupCall(
        RequestInfo requestInfo,
        int date)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

        if (_state.IsDiscarded)
            throw new InvalidOperationException("Cannot start a discarded group call");

        if (!_state.ScheduleDate.HasValue)
            throw new InvalidOperationException("Group call is not scheduled");

        Emit(new GroupCallStartedEvent(
            requestInfo,
            _state.CallId,
            date));
    }
    /// <summary>
    /// Toggles group call recording
    /// </summary>
    public void ToggleRecording(
        RequestInfo requestInfo,
        bool start,
        bool video,
        string? title,
        bool? videoPortrait,
        int date)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

        if (_state.IsDiscarded)
            throw new InvalidOperationException("Cannot toggle recording of discarded call");

        Emit(new GroupCallRecordToggledEvent(
            requestInfo,
            _state.CallId,
            start,
            video,
            title,
            videoPortrait,
            date));
    }

    /// <summary>
    /// Edits group call title
    /// </summary>
    public void EditTitle(
        RequestInfo requestInfo,
        string title,
        int date)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

        if (_state.IsDiscarded)
            throw new InvalidOperationException("Cannot edit title of discarded call");

        Emit(new GroupCallTitleEditedEvent(
            requestInfo,
            _state.CallId,
            title,
            date));
    }
}
