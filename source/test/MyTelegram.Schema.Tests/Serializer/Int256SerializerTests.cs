using System.Buffers;
using MyTelegram.Schema.Extensions;

namespace MyTelegram.Schema.Serializer;



public class Int256SerializerTests
{
    [Fact]
    public void DeserializeTest()
    {
        var expectedBytes = "6B5DB1FB13049F1CBC9FE635C66786F220BF979222CF064F1A59DD0C23971293".ToBytes();
        //var stream = new MemoryStream(expectedBytes);
        //var br = new BinaryReader(stream);
        ReadOnlyMemory<byte> buffer = expectedBytes;
        var serializer = CreateSerializer();

        var actualBytes = serializer.Deserialize(ref buffer);

        actualBytes.ShouldBeEquivalentTo(expectedBytes);
    }

    [Fact]
    public void SerializeTest()
    {
        var expectedBytes = "6B5DB1FB13049F1CBC9FE635C66786F220BF979222CF064F1A59DD0C23971293".ToBytes();
        //var stream = new MemoryStream();
        //var bw = new BinaryWriter(stream);
        using var writer = new ArrayPoolBufferWriter<byte>();
        var serializer = CreateSerializer();

        serializer.Serialize(expectedBytes, writer);

        writer.WrittenSpan.ToArray().ShouldBeEquivalentTo(expectedBytes);
    }

    private Int256Serializer CreateSerializer() => new();
}