namespace MyTelegram.Schema;

[AttributeUsage(AttributeTargets.Class)]
public class TlObjectAttribute(uint constructorId) : Attribute
{
    public uint ConstructorId { get; } = constructorId;
}