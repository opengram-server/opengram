using MyTelegram.Schema.Extensions;

namespace MyTelegram.Schema.Serializer;

//public abstract class SerializeTestBase
//{
//    protected void SerializeTest<TData>(TData t, byte[] expectedValue)
//    {

//    }
//}

public class UInt32SerializerTests
{
    [Fact]
    public void SerializeTest()
    {
        var expectedBytes = "09000080".ToBytes();
        //var stream = new MemoryStream();
        //var bw = new BinaryWriter(stream);
        var value = int.MaxValue + 10u;
        using var writer = new ArrayPoolBufferWriter<byte>();
        var serializer = CreateSerializer();

        serializer.Serialize(value, writer);

        writer.WrittenSpan.ToArray().ShouldBeEquivalentTo(expectedBytes);
    }

    [Fact]
    public void DeserializeTest()
    {
        var expectedBytes = "09000080".ToBytes();
        //var stream = new MemoryStream(expectedBytes);
        //var br = new BinaryReader(stream);
        //var buffer = new ReadOnlySequence<byte>(expectedBytes);
        var expectedValue = int.MaxValue + 10u;
        var serializer = CreateSerializer();
        ReadOnlyMemory<byte> buffer = expectedBytes;

        var actualBytes = serializer.Deserialize(ref buffer);

        actualBytes.ShouldBeEquivalentTo(expectedValue);
    }

    private UInt32Serializer CreateSerializer() => new();
}