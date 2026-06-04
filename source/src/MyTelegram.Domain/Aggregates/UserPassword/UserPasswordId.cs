namespace MyTelegram.Domain.Aggregates.UserPassword;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<UserPasswordId>))]
public class UserPasswordId(string value) : Identity<UserPasswordId>(value)
{
    public static UserPasswordId Create(long userId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"userpassword_{userId}");
    }
}
