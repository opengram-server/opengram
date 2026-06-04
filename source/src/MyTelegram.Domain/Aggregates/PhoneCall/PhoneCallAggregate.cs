using MyTelegram.Domain.Shared;
using MyTelegram.Domain.Events.PhoneCall;

namespace MyTelegram.Domain.Aggregates.PhoneCall;

public class PhoneCallAggregate : AggregateRoot<PhoneCallAggregate, PhoneCallId>
{
    private readonly PhoneCallState _state = new();

    public PhoneCallAggregate(PhoneCallId id) : base(id)
    {
        Register(_state);
    }

    /// <summary>
    /// Gets the current state of the phone call
    /// </summary>
    public PhoneCallState State => _state;

    /// <summary>
    /// Initiates a phone call request (caller side)
    /// </summary>
    public void RequestCall(
        RequestInfo requestInfo,
        long callId,
        long accessHash,
        long adminId,
        long participantId,
        bool isVideo,
        byte[] gAHash,
        PhoneCallProtocol protocol,
        int date)
    {
        Specs.AggregateIsNew.ThrowDomainErrorIfNotSatisfied(this);
        
        if (gAHash == null || gAHash.Length != 32)
            throw new InvalidOperationException("g_a_hash must be 32 bytes (SHA256)");

        Emit(new PhoneCallRequestedEvent(
            requestInfo,
            callId,
            accessHash,
            adminId,
            participantId,
            isVideo,
            gAHash,
            protocol,
            date));
    }

    /// <summary>
    /// Accepts an incoming phone call (callee side)
    /// </summary>
    public void AcceptCall(
        RequestInfo requestInfo,
        byte[] gB,
        PhoneCallProtocol protocol)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        if (_state.Status != PhoneCallStatus.Requested && _state.Status != PhoneCallStatus.Waiting)
            throw new InvalidOperationException($"Cannot accept call in status: {_state.Status}");

        if (gB == null || gB.Length != 256)
            throw new InvalidOperationException("g_b must be 256 bytes");

        Emit(new PhoneCallAcceptedEvent(
            requestInfo,
            _state.CallId,
            _state.AccessHash,
            _state.AdminId,
            _state.ParticipantId,
            gB,
            protocol,
            _state.IsVideo));
    }

    /// <summary>
    /// Confirms the phone call with g_a and key fingerprint (caller side)
    /// </summary>
    public void ConfirmCall(
        RequestInfo requestInfo,
        byte[] gA,
        long keyFingerprint,
        PhoneCallProtocol protocol,
        List<PhoneConnectionInfo> connections,
        int startDate)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        if (_state.Status != PhoneCallStatus.Accepted)
            throw new InvalidOperationException($"Cannot confirm call in status: {_state.Status}");

        if (gA == null || gA.Length != 256)
            throw new InvalidOperationException("g_a must be 256 bytes");

        // Verify that SHA256(gA) matches the previously sent gAHash
        var computedHash = System.Security.Cryptography.SHA256.HashData(gA);
        if (!computedHash.SequenceEqual(_state.GAHash!))
            throw new InvalidOperationException("g_a does not match g_a_hash");

        Emit(new PhoneCallConfirmedEvent(
            requestInfo,
            _state.CallId,
            _state.AccessHash,
            _state.AdminId,
            _state.ParticipantId,
            _state.IsVideo,
            gA,
            keyFingerprint,
            protocol,
            connections,
            startDate));
    }

    /// <summary>
    /// Discards/ends the phone call
    /// </summary>
    public void DiscardCall(
        RequestInfo requestInfo,
        PhoneCallDiscardReason reason,
        int? duration,
        int date,
        bool isVideo)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        if (_state.Status == PhoneCallStatus.Discarded)
            throw new InvalidOperationException("Call already discarded");

        Emit(new PhoneCallDiscardedEvent(
            requestInfo,
            _state.CallId,
            _state.AdminId,
            _state.ParticipantId,
            reason,
            duration,
            date,
            isVideo));
    }

    /// <summary>
    /// Receives signaling data during the call
    /// </summary>
    public void ReceiveSignalingData(
        RequestInfo requestInfo,
        byte[] data)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        if (_state.Status != PhoneCallStatus.Confirmed)
            throw new InvalidOperationException($"Cannot send signaling data in status: {_state.Status}");

        Emit(new PhoneCallSignalingDataReceivedEvent(
            requestInfo,
            _state.CallId,
            data));
    }

    /// <summary>
    /// Marks call as received (callee acknowledges notification)
    /// </summary>
    public void MarkAsReceived(RequestInfo requestInfo, int receiveDate)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        if (_state.Status != PhoneCallStatus.Requested && _state.Status != PhoneCallStatus.Waiting)
            throw new InvalidOperationException($"Cannot mark as received in status: {_state.Status}");

        Emit(new PhoneCallReceivedEvent(
            requestInfo,
            _state.CallId,
            receiveDate));
    }
}
