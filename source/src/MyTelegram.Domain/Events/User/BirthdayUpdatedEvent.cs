namespace MyTelegram.Domain.Events.User;

public class BirthdayUpdatedEvent(Birthday? birthday) : AggregateEvent<UserAggregate, UserId>
{
    public Birthday? Birthday { get; } = birthday;
}