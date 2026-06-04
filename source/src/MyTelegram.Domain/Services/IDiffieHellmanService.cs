namespace MyTelegram.Domain.Services;

/// <summary>
/// Service for Diffie-Hellman key exchange in phone calls
/// Following Telegram's security guidelines: https://core.telegram.org/mtproto/security_guidelines
/// </summary>
public interface IDiffieHellmanService
{
    /// <summary>
    /// Gets the DH configuration (prime p and generator g)
    /// </summary>
    DhConfig GetDhConfig();
    
    /// <summary>
    /// Generates a random value 'a' and computes g_a = g^a mod p
    /// </summary>
    /// <returns>Tuple of (a, g_a)</returns>
    (byte[] a, byte[] gA) GenerateGAAndA(DhConfig config);
    
    /// <summary>
    /// Computes g_b = g^b mod p
    /// </summary>
    byte[] GenerateGB(byte[] b, DhConfig config);
    
    /// <summary>
    /// Computes the shared key = g_b^a mod p (or g_a^b mod p)
    /// </summary>
    byte[] ComputeSharedKey(byte[] gPow, byte[] secret, DhConfig config);
    
    /// <summary>
    /// Computes key fingerprint (lower 64 bits of SHA1(key))
    /// </summary>
    long ComputeKeyFingerprint(byte[] key);
    
    /// <summary>
    /// Validates g, g_a or g_b according to security guidelines
    /// </summary>
    bool ValidateDhValue(byte[] value, DhConfig config);
    
    /// <summary>
    /// Validates the DH prime p
    /// </summary>
    bool ValidatePrime(byte[] prime);
}

public record DhConfig(byte[] Prime, int Generator, int Version);
