using System.Security.Cryptography;

namespace MyTelegram.Core;

public class HashHelper : IHashHelper, ISingletonDependency
{
    public byte[] Md5(ReadOnlySpan<byte> source)
    {
        return MD5.HashData(source);
    }

    public byte[] Sha1(ReadOnlySpan<byte> source)
    {
        return SHA1.HashData(source);
        //using var sha1 = SHA1.Create();

        //return sha1.ComputeHash(data);
    }

    public byte[] Sha256(ReadOnlySpan<byte> source)
    {
        return SHA256.HashData(source);
    }

    public byte[] Sha256(byte[] inputBuffer1, byte[] inputBuffer2)
    {
        var sha2 = SHA256.Create();
        sha2.TransformBlock(inputBuffer1, 0, inputBuffer1.Length, null, 0);
        sha2.TransformBlock(inputBuffer2, 0, inputBuffer2.Length, null, 0);
        sha2.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

        return sha2.Hash!;
    }

    public byte[] Sha512(ReadOnlySpan<byte> source)
    {
        return SHA512.HashData(source);
    }
}