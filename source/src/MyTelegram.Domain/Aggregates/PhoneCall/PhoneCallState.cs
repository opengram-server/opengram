using MyTelegram.Domain.Events.PhoneCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Aggregates.PhoneCall;

public class PhoneCallState : AggregateState<PhoneCallAggregate, PhoneCallId, PhoneCallState>,
    IApply<PhoneCallRequestedEvent>,
    IApply<PhoneCallAcceptedEvent>,
    IApply<PhoneCallConfirmedEvent>,
    IApply<PhoneCallDiscardedEvent>,
    IApply<PhoneCallSignalingDataReceivedEvent>,
    IApply<PhoneCallReceivedEvent>
{
    public long CallId { get; private set; }
    public long AccessHash { get; private set; }
    public long AdminId { get; private set; }
    public long ParticipantId { get; private set; }
    public bool IsVideo { get; private set; }
    public PhoneCallStatus Status { get; private set; }
    
    // Diffie-Hellman key exchange data
    public byte[]? GAHash { get; private set; }
    public byte[]? GA { get; private set; }
    public byte[]? GB { get; private set; }
    public long? KeyFingerprint { get; private set; }
    
    // Protocol and connections
    public PhoneCallProtocol? Protocol { get; private set; }
    public List<PhoneConnectionInfo> Connections { get; private set; } = new();
    
    // Timestamps
    public int RequestDate { get; private set; }
    public int? ReceiveDate { get; private set; }
    public int? StartDate { get; private set; }
    public int? DiscardDate { get; private set; }
    
    // Discard info
    public PhoneCallDiscardReason? DiscardReason { get; private set; }
    public int? Duration { get; private set; }

    public void Apply(PhoneCallRequestedEvent aggregateEvent)
    {
        CallId = aggregateEvent.CallId;
        AccessHash = aggregateEvent.AccessHash;
        AdminId = aggregateEvent.AdminId;
        ParticipantId = aggregateEvent.ParticipantId;
        IsVideo = aggregateEvent.IsVideo;
        GAHash = aggregateEvent.GAHash;
        Protocol = aggregateEvent.Protocol;
        RequestDate = aggregateEvent.Date;
        Status = PhoneCallStatus.Requested;
    }

    public void Apply(PhoneCallAcceptedEvent aggregateEvent)
    {
        GB = aggregateEvent.GB;
        Protocol = aggregateEvent.Protocol;
        Status = PhoneCallStatus.Accepted;
    }

    public void Apply(PhoneCallConfirmedEvent aggregateEvent)
    {
        GA = aggregateEvent.GA;
        KeyFingerprint = aggregateEvent.KeyFingerprint;
        Connections = aggregateEvent.Connections;
        StartDate = aggregateEvent.StartDate;
        Status = PhoneCallStatus.Confirmed;
    }

    public void Apply(PhoneCallDiscardedEvent aggregateEvent)
    {
        Status = PhoneCallStatus.Discarded;
        DiscardReason = aggregateEvent.Reason;
        Duration = aggregateEvent.Duration;
        DiscardDate = aggregateEvent.Date;
    }

    public void Apply(PhoneCallSignalingDataReceivedEvent aggregateEvent)
    {
        // Signaling data is processed but not stored in state
    }

    public void Apply(PhoneCallReceivedEvent aggregateEvent)
    {
        ReceiveDate = aggregateEvent.ReceiveDate;
    }
}

public enum PhoneCallStatus
{
    Empty = 0,
    Waiting = 1,
    Requested = 2,
    Accepted = 3,
    Confirmed = 4,
    Discarded = 5
}

