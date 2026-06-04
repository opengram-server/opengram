namespace MyTelegram.Schema;

public interface ISerializer<T>
{
    void Serialize(T value,
        IBufferWriter<byte> writer);

    T Deserialize(ref ReadOnlyMemory<byte> buffer);
}