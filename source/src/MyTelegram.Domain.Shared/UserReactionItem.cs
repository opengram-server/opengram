namespace MyTelegram;

public record UserReactionItem(long UserId,
    List<long> ReactionIds);