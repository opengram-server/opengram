namespace MyTelegram;

public record SendMessageItem(MessageItem MessageItem, bool ClearDraft = false, List<long>? MentionedUserIds = null, List<long>? ChatMembers = null, int? SenderDefaultHistoryTTL = null);