namespace MyTelegram.Core;

public interface IMtpHelper
{
    void CalcTempAesKeyData(byte[] newNonce,
        byte[] serverNonce, Span<byte> aesKey, Span<byte> aesIv);

    long ComputeSalt(byte[] newNonce,
        byte[] serverNonce);

    void Encrypt(long authKeyId, byte[] authKeyData, ReadOnlySpan<byte> data, Span<byte> outputBuffer);
}