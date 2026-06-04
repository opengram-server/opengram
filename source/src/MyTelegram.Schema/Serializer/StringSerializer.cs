namespace MyTelegram.Schema.Serializer;

public class StringSerializer : ISerializer<string>
{
    private readonly BytesSerializer _bytesSerializer = new();


    public void Serialize(string value,
        IBufferWriter<byte> writer)
    {
        using var owner = MemoryPool<byte>.Shared.Rent(value.Length * 4);
        var count = Encoding.UTF8.GetBytes(value, owner.Memory.Span);
        var buffer = owner.Memory.Slice(0, count);
        _bytesSerializer.Serialize(buffer, writer);
    }

    public string Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        if (buffer.Length == 0)
        {
            return string.Empty;
        }
        
        var firstByte = buffer.Span[0];
        buffer = buffer[1..];
        var length = 0;
        var padding = 0;

        if (firstByte == 254)
        {
            if (buffer.Length > 3)
            {
                var lengthBytes = buffer.Slice(0, 3).Span;
                length = lengthBytes[0] | (lengthBytes[1] << 8) | lengthBytes[2] << 16;
                padding = length % 4;
            }
            else
            {
                throw new ArgumentException("Read buffer length failed");
            }

            buffer = buffer[3..];
        }
        else
        {
            length = firstByte;
            padding = (length + 1) % 4;
        }

        if (length > buffer.Length)
        {
            throw new ArgumentException($"String length {length} exceeds buffer length {buffer.Length}");
        }

        //var sequence = reader.UnreadSequence.Slice(0, length);
        //var text = Encoding.UTF8.GetString(sequence);
        var textSpan = buffer.Slice(0, length).Span;
        var text = Encoding.UTF8.GetString(textSpan);

        //reader.Advance(length);
        buffer = buffer[length..];

        if (padding > 0)
        {
            padding = 4 - padding;
            //reader.Advance(padding);
            buffer = buffer[padding..];
        }

        return text;
    }
}