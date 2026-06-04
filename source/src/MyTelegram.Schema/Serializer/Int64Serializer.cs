using System.Buffers.Binary;

namespace MyTelegram.Schema.Serializer;

//ReSharper disable once CommentTypo
/// <summary>
/// Values of type long are two-element sequences that are 64-bit signed numbers (little endian again)
/// </summary>
public class Int64Serializer : ISerializer<long>
{
    public void Serialize(long value,
        IBufferWriter<byte> writer)
    {
        writer.Write(value);
    }

    public long Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        var value = BinaryPrimitives.ReadInt64LittleEndian(buffer.Span);
        buffer = buffer[8..];

        return value;
    }
}