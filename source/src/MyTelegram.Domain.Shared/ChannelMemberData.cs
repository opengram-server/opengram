namespace MyTelegram;

//public record ChannelMemberData(long UserId, long AccessHashKeyId, int BannedRights, int UntilDate);
public record LayeredResponseExtraData(int BannedRights, int UntilDate);