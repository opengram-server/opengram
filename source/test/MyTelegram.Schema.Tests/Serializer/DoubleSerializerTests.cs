using System.Buffers;
using MyTelegram.Schema.Extensions;

namespace MyTelegram.Schema.Serializer;

public class DoubleSerializerTests
{
    [InlineData(1.2345d)]
    [InlineData(-1.2345d)]
    [InlineData(10000.2345d)]
    [Theory]
    public void SerializeTest(double a)
    {
        var bytes = BitConverter.GetBytes(a);
        //var stream = new MemoryStream();
        //var bw = new BinaryWriter(stream);
        using var writer = new ArrayPoolBufferWriter<byte>();
        var serializer = CreateSerializer();

        serializer.Serialize(a, writer);

        var actualBytes = writer.WrittenSpan.ToArray();
        actualBytes.ShouldBeEquivalentTo(bytes);
    }


    [InlineData("8D976E1283C0F33F", 1.2345d)]
    [InlineData("8D976E1283C0F3BF", -1.2345d)]
    [InlineData("759318041E88C340", 10000.2345d)]
    [Theory]
    public void DeserializeTest(string data, double expectedValue)
    {
        var dataBytes = data.ToBytes();
        //var br = new BinaryReader(new MemoryStream(dataBytes));
        var serializer = CreateSerializer();
        ReadOnlyMemory<byte> buffer = dataBytes;

        var actualValue = serializer.Deserialize(ref buffer);

        actualValue.ShouldBe(expectedValue);
    }

    private DoubleSerializer CreateSerializer() => new();
}