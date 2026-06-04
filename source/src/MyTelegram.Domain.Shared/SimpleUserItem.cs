namespace MyTelegram;

public record SimpleUserItem(long SelfUserId, bool IsPremium, bool IsBot = false);