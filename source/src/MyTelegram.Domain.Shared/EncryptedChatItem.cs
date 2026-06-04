namespace MyTelegram;

public record EncryptedChatItem(int ChatId, long AccessHash,
    long AdminId,
    long ParticipantId,
    //long AdminAuthKeyId,
    long AdminPermAuthKeyId,
    //long ParticipantAuthKeyId,
    long ParticipantPermAuthKeyId
);