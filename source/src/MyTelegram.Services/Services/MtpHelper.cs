using System.Buffers;
using System.Buffers.Binary;
using System.Security.Cryptography;

namespace MyTelegram.Services.Services;

public class MtpHelper(IAesHelper aesHelper) : IMtpHelper, ITransientDependency
{
    public void Encrypt(long authKeyId, byte[] authKeyData, ReadOnlySpan<byte> data, Span<byte> outputBuffer)
    {
        Span<byte> tempSpan = stackalloc byte[32];
        Span<byte> messageKey = tempSpan.Slice(0, 16);
        CalcMessageKey(authKeyData, data, true, messageKey);
        //Span<byte> aesKey = tempSpan.Slice(16, 32);
        var aesKey = new byte[32];
        Span<byte> aesIv = tempSpan.Slice(16, 16);
        CalcAesKey(authKeyData, messageKey, false, aesKey, aesIv);

        var encryptedSpan = outputBuffer.Slice(24);
        aesHelper.EncryptIge(data, aesKey, aesIv, encryptedSpan);
        BinaryPrimitives.WriteInt64LittleEndian(outputBuffer, authKeyId);
        messageKey.CopyTo(outputBuffer.Slice(8, 16));
    }

    public void CalcTempAesKeyData(byte[] newNonce,
        byte[] serverNonce, Span<byte> aesKey, Span<byte> aesIv)
    {
        // serverNonce.Length=16 newNonce.Length=32
        // tmp_aes_key := SHA1(new_nonce + server_nonce) + substr (SHA1(server_nonce + new_nonce), 0, 12);
        // tmp_aes_iv := substr (SHA1(server_nonce + new_nonce), 12, 8) + SHA1(new_nonce + new_nonce) + substr (new_nonce, 0, 4);
        // encrypted_answer := AES256_ige_encrypt (answer_with_hash, tmp_aes_key, tmp_aes_iv); here, tmp_aes_key is a 256-bit key, and tmp_aes_iv is a 256-bit initialization vector. The same as in all the other instances that use AES encryption, the encrypted data is padded with random bytes to a length divisible by 16 immediately prior to encryption.

        // https://corefork.telegram.org/mtproto/auth_key
        // newNonce is int256,serverNonce is int128
        var length = newNonce.Length + serverNonce.Length;
        var tempBytes = ArrayPool<byte>.Shared.Rent(48 + 48 + 64 + 20 + 20 + 20);
        var tempSpan = tempBytes.AsSpan();

        try
        {
            //Span<byte> ns = stackalloc byte[length];
            //Span<byte> sn = stackalloc byte[length];
            //Span<byte> nn = stackalloc byte[newNonce.Length + newNonce.Length];
            var ns = tempSpan.Slice(0, 48);
            var sn = tempSpan.Slice(48, 48);
            var nn = tempSpan.Slice(96, 64);

            newNonce.CopyTo(ns);
            serverNonce.CopyTo(ns[newNonce.Length..]);

            serverNonce.CopyTo(sn);
            newNonce.CopyTo(sn[serverNonce.Length..]);

            newNonce.CopyTo(nn);
            newNonce.CopyTo(nn[newNonce.Length..]);

            //var nsHash = hashHelper.Sha1(ns);
            //var snHash = hashHelper.Sha1(sn);
            //var nnHash = hashHelper.Sha1(nn);
            var nsHash = tempSpan.Slice(160, 20);
            var snHash = tempSpan.Slice(180, 20);
            var nnHash = tempSpan.Slice(200, 20);
            SHA1.HashData(ns, nsHash);
            SHA1.HashData(sn, snHash);
            SHA1.HashData(nn, nnHash);

            nsHash.CopyTo(aesKey);
            snHash.Slice(0, 12).CopyTo(aesKey[nsHash.Length..]);
            snHash.Slice(12, 8).CopyTo(aesIv);
            nnHash.CopyTo(aesIv[8..]);
            newNonce.AsSpan(0, 4).CopyTo(aesIv[(8 + nnHash.Length)..]);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(tempBytes);
        }
    }

    public long ComputeSalt(byte[] newNonce,
        byte[] serverNonce)
    {
        Span<byte> bytes = stackalloc byte[8];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte)(newNonce[i] ^ serverNonce[i]);
        }

        return BitConverter.ToInt64(bytes);
    }

    private void CalcAesKey(ReadOnlySpan<byte> authKey, ReadOnlySpan<byte> msgKey, bool toServer, Span<byte> aesKey,
        Span<byte> aesIv)
    {
        if (msgKey.Length != 16)
        {
            throw new ArgumentException($"MsgKey length:{msgKey.Length} should be 16.");
        }

        var x = toServer ? 0 : 8;
        // Length=52(msgKey.Length=16)+36
        // length=168
        Span<byte> span = stackalloc byte[52 + 52 + 32 + 32];
        var aSource = span[..52];
        var bSource = span.Slice(52, 52);
        var sha256A = span.Slice(104, 32);
        var sha256B = span.Slice(136, 32);

        msgKey.CopyTo(aSource);
        authKey.Slice(x, 36).CopyTo(aSource[16..]);
        authKey.Slice(40 + x, 36).CopyTo(bSource);
        msgKey.CopyTo(bSource[36..]);

        SHA256.HashData(aSource, sha256A);
        SHA256.HashData(bSource, sha256B);
        sha256A[..8].CopyTo(aesKey);
        sha256B.Slice(8, 16).CopyTo(aesKey[8..]);
        sha256A.Slice(24, 8).CopyTo(aesKey[24..]);

        sha256B[..8].CopyTo(aesIv);
        sha256A.Slice(8, 16).CopyTo(aesIv[8..]);
        sha256B.Slice(24, 8).CopyTo(aesIv[24..]);
    }

    /// <summary>
    /// Calc message key (16 bytes)
    /// </summary>
    /// <param name="authKey"></param>
    /// <param name="data"></param>
    /// <param name="toServer"></param>
    /// <param name="messageKey">16 bytes message key</param>
    private void CalcMessageKey(ReadOnlySpan<byte> authKey, ReadOnlySpan<byte> data, bool toServer, Span<byte> messageKey)
    {
        var x = toServer ? 8 : 0;
        var bytes = ArrayPool<byte>.Shared.Rent(32 + data.Length + 32);
        var tempSpan = bytes.AsSpan();
        try
        {
            var span = tempSpan[..(32 + data.Length)];
            authKey.Slice(88 + x, 32).CopyTo(span);
            data.CopyTo(span.Slice(32));
            var messageKeyHash = tempSpan.Slice(32 + data.Length, 32);
            SHA256.HashData(span, messageKeyHash);
            messageKeyHash.Slice(8, 16).CopyTo(messageKey);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(bytes);
        }
    }
}
