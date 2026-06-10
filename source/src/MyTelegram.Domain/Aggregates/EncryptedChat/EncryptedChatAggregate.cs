using MyTelegram.Domain.Events.EncryptedChat;

namespace MyTelegram.Domain.Aggregates.EncryptedChat;

public class EncryptedChatAggregate : AggregateRoot<EncryptedChatAggregate, EncryptedChatId>
{
    private readonly EncryptedChatState _state = new();

    public EncryptedChatAggregate(EncryptedChatId id) : base(id)
    {
        Register(_state);
    }

    public EncryptedChatState State => _state;

    /// <summary>
    /// Initiates a secret chat request (User A sends g_a to User B)
    /// </summary>
    public void RequestEncryption(
        RequestInfo requestInfo,
        int chatId,
        long accessHash,
        long adminId,
        long participantId,
        long adminPermAuthKeyId,
        byte[] gA,
        int date)
    {
        Specs.AggregateIsNew.ThrowDomainErrorIfNotSatisfied(this);

        Emit(new EncryptionRequestedEvent(
            requestInfo,
            chatId,
            accessHash,
            adminId,
            participantId,
            adminPermAuthKeyId,
            gA,
            date));
    }

    /// <summary>
    /// Accepts a secret chat (User B sends g_b and key fingerprint)
    /// </summary>
    public void AcceptEncryption(
        RequestInfo requestInfo,
        byte[] gB,
        long keyFingerprint,
        long participantPermAuthKeyId)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

        if (_state.Status != EncryptedChatStatus.Requested)
            throw new InvalidOperationException($"Cannot accept encryption in status: {_state.Status}");

        Emit(new EncryptionAcceptedEvent(
            requestInfo,
            _state.ChatId,
            _state.AccessHash,
            _state.AdminId,
            _state.ParticipantId,
            gB,
            keyFingerprint,
            participantPermAuthKeyId,
            _state.GA!));
    }

    /// <summary>
    /// Discards/cancels the secret chat
    /// </summary>
    public void DiscardEncryption(
        RequestInfo requestInfo,
        bool deleteHistory)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

        if (_state.Status == EncryptedChatStatus.Discarded)
            throw new InvalidOperationException("Encryption already discarded");

        Emit(new EncryptionDiscardedEvent(
            requestInfo,
            _state.ChatId,
            _state.AdminId,
            _state.ParticipantId,
            deleteHistory));
    }

    /// <summary>
    /// Records an encrypted message being sent (for event-driven relay)
    /// </summary>
    public void SendEncryptedMessage(
        RequestInfo requestInfo,
        long randomId,
        byte[] data,
        byte[]? fileData,
        SendMessageType messageType,
        int date)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

        if (_state.Status != EncryptedChatStatus.Accepted)
            throw new InvalidOperationException($"Cannot send encrypted message in status: {_state.Status}");

        Emit(new EncryptedMessageSentEvent(
            requestInfo,
            _state.ChatId,
            _state.AccessHash,
            _state.AdminId,
            _state.ParticipantId,
            _state.AdminPermAuthKeyId,
            _state.ParticipantPermAuthKeyId,
            randomId,
            data,
            fileData,
            messageType,
            date));
    }
}
