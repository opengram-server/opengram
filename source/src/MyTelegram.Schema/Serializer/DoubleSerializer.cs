using System.Buffers.Binary;

namespace MyTelegram.Schema.Serializer;

/// <summary>
/// Values of type double, are two-element sequences containing 64-bit real numbers in a standard double format
/// </summary>
public class DoubleSerializer : ISerializer<double>//, ISerializer2<double>
{
    public void Serialize(double value,
        IBufferWriter<byte> writer)
    {
        writer.Write(value);
    }

    public double Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        var value = BinaryPrimitives.ReadDoubleLittleEndian(buffer.Span);
        buffer = buffer[8..];

        return value;
    }
}