namespace MyTelegram.Core;

public interface IMyRsaHelper
{
    byte[] Decrypt(ReadOnlySpan<byte> encryptedSpan,
        string privateKey);

    //long GetFingerprint(string publicKeyWithFormat);
    long GetFingerprintFromPrivateKey(string privateKeyWithFormat);
}