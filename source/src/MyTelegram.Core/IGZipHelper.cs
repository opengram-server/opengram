using System.Buffers;

namespace MyTelegram.Core;

public interface IGZipHelper
{
    void Compress(ReadOnlySpan<byte> source, IBufferWriter<byte> writer);
    void Decompress(ReadOnlyMemory<byte> source, Span<byte> destination, out int count);
    bool TryGetUncompressedLength(ReadOnlySpan<byte> data, out int length);
}