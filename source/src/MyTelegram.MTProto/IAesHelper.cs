namespace MyTelegram.MTProto;

public interface IAesHelper
{
    void DecryptIge(ReadOnlySpan<byte> input,
        byte[] outputBytes,
        byte[] key,
        byte[] iv);

    void EncryptIge(ReadOnlySpan<byte> input,
        byte[] outputBytes,
        byte[] key,
        byte[] iv);

    void CtrEncrypt(ReadOnlySpan<byte> input, Span<byte> output, byte[] key, byte[] iv,
        ulong offset = 0);
}
