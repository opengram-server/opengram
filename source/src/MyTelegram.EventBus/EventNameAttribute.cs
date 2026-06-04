namespace MyTelegram.EventBus;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class EventNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}