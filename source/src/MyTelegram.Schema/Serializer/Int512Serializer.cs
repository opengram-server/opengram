namespace MyTelegram.Schema.Serializer;

/// <summary>
/// int512 16*[ int ] = Int512;
/// </summary>
public class Int512Serializer : ISerializer<byte[]>
{
    public void Serialize(byte[] value, IBufferWriter<byte> writer)
    {
        writer.WriteRawBytes(value);
    }

    public byte[] Deserialize(ref SequenceReader<byte> reader)
    {
        var data = new byte[64];
        reader.TryCopyTo(data);
        reader.Advance(64);

        return data;
    }

    public byte[] Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        var value = buffer.Slice(0, 64);
        buffer = buffer[64..];

        return value.ToArray();
    }
}