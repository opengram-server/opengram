namespace MyTelegram.Core;
public record ChannelMemberBannedEvent(long ChannelId,long UserId,int BannedRights,int UntilDate);