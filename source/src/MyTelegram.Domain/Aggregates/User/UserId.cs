namespace MyTelegram.Domain.Aggregates.User;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<UserId>))]
public class UserId(string value) : Identity<UserId>(value)
{
    public static UserId Create(long userId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"user_{userId}");
    }
}
