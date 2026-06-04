namespace MyTelegram;

public record JoinChatlistItem(long ChannelId, bool Broadcast, int TopMessageId, int ChannelHistoryMinId);