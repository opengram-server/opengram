namespace MyTelegram.Schema.Serializer;

public class Int64SerializerTests
{
    [Fact]
    public void SerializeTest()
    {
        var expectedBytes = "6300000000000000".ToBytes();
        //var stream = new MemoryStream();
        //var bw = new BinaryWriter(stream);
        using var writer = new ArrayPoolBufferWriter<byte>();
        var serializer = CreateSerializer();

        serializer.Serialize(99L, writer);

        writer.WrittenSpan.ToArray().ShouldBeEquivalentTo(expectedBytes);
    }

    [Fact]
    public void DeserializeTest()
    {
        var expectedBytes = "6300000000000000".ToBytes();
        ReadOnlyMemory<byte> buffer = expectedBytes;
        var serializer = CreateSerializer();

        var actualBytes = serializer.Deserialize(ref buffer);

        actualBytes.ShouldBeEquivalentTo(99L);
    }

    private Int64Serializer CreateSerializer() => new();
}