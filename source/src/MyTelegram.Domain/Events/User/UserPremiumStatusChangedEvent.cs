namespace MyTelegram.Domain.Events.User;

public class UserPremiumStatusChangedEvent(long userId, string phoneNumber, bool premium)
    : AggregateEvent<UserAggregate, UserId>
{
    public long UserId { get; } = userId;
    public string PhoneNumber { get; } = phoneNumber;
    public bool Premium { get; } = premium;
}