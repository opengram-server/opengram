namespace MyTelegram.Schema.Serializer;

public class BitArraySerializer : ISerializer<BitArray>
{
    public void Serialize(BitArray value,
        IBufferWriter<byte> writer)
    {
        var data = new byte[(value.Length - 1) / 8 + 1];
        value.CopyTo(data, 0);
        writer.WriteRawBytes(data);
    }

    public BitArray Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        var data = new byte[4];
        buffer.CopyTo(data);
        var value = new BitArray(data);
        buffer = buffer[4..];
        
        return value;
    }
}