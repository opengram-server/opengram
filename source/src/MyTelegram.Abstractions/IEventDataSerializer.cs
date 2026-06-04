namespace MyTelegram.Abstractions;

public interface IEventDataSerializer<TEventData> : IEventDataSerializer
{
    void Serialize(IBufferWriter<byte> writer, TEventData data);
    TEventData Deserialize(ReadOnlyMemory<byte> buffer);
}

public interface IEventDataSerializer
{
    //object? Deserialize(Type type, ReadOnlyMemory<byte> buffer);
    object Deserialize(Type type, ReadOnlyMemory<byte> buffer);
}