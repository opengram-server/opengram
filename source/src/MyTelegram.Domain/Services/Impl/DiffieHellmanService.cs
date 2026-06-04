using System.Numerics;
using System.Security.Cryptography;

namespace MyTelegram.Domain.Services.Impl;

/// <summary>
/// Implementation of Diffie-Hellman key exchange for Telegram phone calls
/// Based on: https://core.telegram.org/api/end-to-end/voice-calls
/// </summary>
public class DiffieHellmanService : IDiffieHellmanService
{
    // Telegram uses a 2048-bit DH prime
    // This is a safe prime from messages.getDhConfig
    private static readonly byte[] DefaultPrime = Convert.FromHexString(
        "C71CAEB9C6B1C9048E6C522F70F13F73980D40238E3E21C14934D037563D930F" +
        "48198A0AA7C14058229493D22530F4DBFA336F6E0AC925139543AED44CCE7C37" +
        "20FD51F69458705AC68CD4FE6B6B13ABDC9746512969328454F18FAF8C595F64" +
        "2477FE96BB2A941D5BCD1D4AC8CC49880708FA9B378E3C4F3A9060BEE67CF9A4" +
        "A4A695811051907E162753B56B0F6B410DBA74D8A84B2A14B3144E0EF1284754" +
        "FD17ED950D5965B4B9DD46582DB1178D169C6BC465B0D6FF9CA3928FEF5B9AE4" +
        "E418FC15E83EBEA0F87FA9FF5EED70050DED2849F47BF959D956850CE929851F" +
        "0D8115F635B105EE2E4E15D04B2454BF6F4FADF034B10403119CD8E3B92FCC5B");

    private const int DefaultGenerator = 3;
    private const int DhConfigVersion = 1;

    public DhConfig GetDhConfig()
    {
        return new DhConfig(DefaultPrime, DefaultGenerator, DhConfigVersion);
    }

    public (byte[] a, byte[] gA) GenerateGAAndA(DhConfig config)
    {
        var prime = new BigInteger(config.Prime, isUnsigned: true, isBigEndian: true);
        var g = new BigInteger(config.Generator);
        
        // Generate random 'a' where 1 < a < p-1
        var a = GenerateRandomSecret(config.Prime.Length);
        var aBig = new BigInteger(a, isUnsigned: true, isBigEndian: true);
        
        // Compute g_a = g^a mod p
        var gA = BigInteger.ModPow(g, aBig, prime);
        
        return (a, gA.ToByteArray(isUnsigned: true, isBigEndian: true));
    }

    public byte[] GenerateGB(byte[] b, DhConfig config)
    {
        var prime = new BigInteger(config.Prime, isUnsigned: true, isBigEndian: true);
        var g = new BigInteger(config.Generator);
        var bBig = new BigInteger(b, isUnsigned: true, isBigEndian: true);
        
        // Compute g_b = g^b mod p
        var gB = BigInteger.ModPow(g, bBig, prime);
        
        return gB.ToByteArray(isUnsigned: true, isBigEndian: true);
    }

    public byte[] ComputeSharedKey(byte[] gPow, byte[] secret, DhConfig config)
    {
        var prime = new BigInteger(config.Prime, isUnsigned: true, isBigEndian: true);
        var gPowBig = new BigInteger(gPow, isUnsigned: true, isBigEndian: true);
        var secretBig = new BigInteger(secret, isUnsigned: true, isBigEndian: true);
        
        // Validate g_pow before computation
        if (!ValidateDhValue(gPow, config))
        {
            throw new InvalidOperationException("Invalid DH value");
        }
        
        // Compute key = (g^pow)^secret mod p = g^(pow*secret) mod p
        var key = BigInteger.ModPow(gPowBig, secretBig, prime);
        
        return key.ToByteArray(isUnsigned: true, isBigEndian: true);
    }

    public long ComputeKeyFingerprint(byte[] key)
    {
        // Compute SHA1 of the key
        var hash = SHA1.HashData(key);
        
        // Take lower 64 bits (last 8 bytes) and convert to long
        return BitConverter.ToInt64(hash, hash.Length - 8);
    }

    public bool ValidateDhValue(byte[] value, DhConfig config)
    {
        var valueBig = new BigInteger(value, isUnsigned: true, isBigEndian: true);
        var prime = new BigInteger(config.Prime, isUnsigned: true, isBigEndian: true);
        
        // Security check: 1 < value < p-1
        if (valueBig <= BigInteger.One)
            return false;
        
        if (valueBig >= prime - BigInteger.One)
            return false;
        
        // Additional security check: 2^{2048-64} < value < p - 2^{2048-64}
        var safetyBound = BigInteger.Pow(2, 2048 - 64);
        if (valueBig <= safetyBound)
            return false;
        
        if (valueBig >= prime - safetyBound)
            return false;
        
        return true;
    }

    public bool ValidatePrime(byte[] prime)
    {
        // In production, this should verify that p is a safe prime (p = 2q + 1 where q is also prime)
        // For now, we trust the Telegram-provided prime
        return prime.Length == 256 && prime.SequenceEqual(DefaultPrime);
    }

    private byte[] GenerateRandomSecret(int length)
    {
        var secret = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(secret);
        
        // Ensure it's in valid range by clearing top bit
        secret[0] &= 0x7F;
        
        return secret;
    }
}
