using System.Buffers.Binary;

namespace MyTelegram.Schema.Serializer;

public class UInt32Serializer : ISerializer<uint>
{
    public void Serialize(uint value,
        IBufferWriter<byte> writer)
    {
        writer.Write(value);
    }

    public uint Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        var value = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Span);
        buffer = buffer[4..];

        return value;
    }
}