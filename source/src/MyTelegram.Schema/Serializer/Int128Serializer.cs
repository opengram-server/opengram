namespace MyTelegram.Schema.Serializer;

/// <summary>
/// int128 4*[ int ] = Int128;
/// </summary>
public class Int128Serializer : ISerializer<byte[]>
{
    public void Serialize(byte[] value,
        IBufferWriter<byte> writer)
    {
        writer.WriteRawBytes(value);
    }

    public byte[] Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        var value = buffer[..16];
        buffer = buffer[16..];

        return value.ToArray();
    }
}