using System;

namespace MyTelegram.Schema.Serializer;

/// <summary>
/// int256 8*[ int ] = Int256;
/// </summary>
public class Int256Serializer : ISerializer<byte[]>
{
    public void Serialize(byte[] value,
        IBufferWriter<byte> writer)
    {
        writer.WriteRawBytes(value);
    }

    public byte[] Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        var value = buffer.Slice(0, 32);
        buffer = buffer[32..];

        return value.ToArray();
    }
}