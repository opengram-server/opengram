namespace MyTelegram.Core;

public interface IAesHelper
{
    void EncryptIge(ReadOnlySpan<byte> source, byte[] key, ReadOnlySpan<byte> iv, Span<byte> destination);
    void DecryptIge(ReadOnlySpan<byte> source, byte[] key, ReadOnlySpan<byte> iv, Span<byte> destination);
    void CtrEncrypt(ReadOnlySpan<byte> input, Span<byte> destination, byte[] key, byte[] iv,
        ulong offset = 0);
    void CtrEncrypt(ReadOnlyMemory<byte> input, Memory<byte> destination, byte[] key, byte[] iv,
        ulong offset = 0);
}