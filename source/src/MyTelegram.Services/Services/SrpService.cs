using System.Numerics;
using System.Security.Cryptography;

namespace MyTelegram.Services.Impl;

public class SrpService : ISrpService, ISingletonDependency
{
    private readonly ILogger<SrpService> _logger;
    private readonly IRandomHelper _randomHelper;

    private const int DefaultG = 3;
    private static readonly byte[] DefaultP = Convert.FromHexString(
        "c71caeb9c6b1c9048e6c522f70f13f73980d40238e3e21c14934d037563d930f48198a0aa7c14058229493d22530f4dbfa336f6e0ac925139543aed44cce7c3720fd51f69458705ac68cd4fe6b6b13abdc9746512969328454f18faf8c595f642477fe96bb2a941d5bcd1d4ac8cc49880708fa9b378e3c4f3a9060bee67cf9a4a4a695811051907e162753b56b0f6b410dba74d8a84b2a14b3144e0ef1284754fd17ed950d5965b4b9dd46582db1178d169c6bc465b0d6ff9ca3928fef5b9ae4e418fc15e83ebea0f87fa9ff5eed70050ded2849f47bf959d956850ce929851f0d8115f635b105ee2e4e15d04b2454bf6f4fadf034b10403119cd8e3b92fcc5b"
    );

    public SrpService(ILogger<SrpService> logger, IRandomHelper randomHelper)
    {
        _logger = logger;
        _randomHelper = randomHelper;
    }

    public byte[] ComputeVerifier(string password, byte[] salt1, byte[] salt2, int g, byte[] p)
    {
        var x = ComputeX(password, salt1, salt2);
        var gBig = new BigInteger(g);
        var pBig = new BigInteger(p, isUnsigned: true, isBigEndian: true);
        var xBig = new BigInteger(x, isUnsigned: true, isBigEndian: true);

        var v = BigInteger.ModPow(gBig, xBig, pBig);
        return v.ToByteArray(isUnsigned: true, isBigEndian: true);
    }

    public byte[] ComputeX(string password, byte[] salt1, byte[] salt2)
    {
        // According to Telegram SRP documentation:
        // SH(data, salt) = H(salt | data | salt)
        // PH1(password, salt1, salt2) = SH(SH(password, salt1), salt2)
        // PH2(password, salt1, salt2) = SH(pbkdf2(sha512, PH1(password, salt1, salt2), salt1, 100000), salt2)
        // x = PH2(password, salt1, salt2)
        
        var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
        
        // Step 1: SH(password, salt1) = H(salt1 | password | salt1)
        var sh1Data = new byte[salt1.Length + passwordBytes.Length + salt1.Length];
        Buffer.BlockCopy(salt1, 0, sh1Data, 0, salt1.Length);
        Buffer.BlockCopy(passwordBytes, 0, sh1Data, salt1.Length, passwordBytes.Length);
        Buffer.BlockCopy(salt1, 0, sh1Data, salt1.Length + passwordBytes.Length, salt1.Length);
        var sh1 = SHA256.HashData(sh1Data);
        
        // Step 2: PH1 = SH(sh1, salt2) = H(salt2 | sh1 | salt2)
        var ph1Data = new byte[salt2.Length + sh1.Length + salt2.Length];
        Buffer.BlockCopy(salt2, 0, ph1Data, 0, salt2.Length);
        Buffer.BlockCopy(sh1, 0, ph1Data, salt2.Length, sh1.Length);
        Buffer.BlockCopy(salt2, 0, ph1Data, salt2.Length + sh1.Length, salt2.Length);
        var ph1 = SHA256.HashData(ph1Data);
        
        // Step 3: pbkdf2(sha512, PH1, salt1, 100000)
        var pbkdf2 = new Rfc2898DeriveBytes(ph1, salt1, 100000, HashAlgorithmName.SHA512);
        var pbkdf2Result = pbkdf2.GetBytes(64);
        
        // Step 4: PH2 = SH(pbkdf2Result, salt2) = H(salt2 | pbkdf2Result | salt2)
        var ph2Data = new byte[salt2.Length + pbkdf2Result.Length + salt2.Length];
        Buffer.BlockCopy(salt2, 0, ph2Data, 0, salt2.Length);
        Buffer.BlockCopy(pbkdf2Result, 0, ph2Data, salt2.Length, pbkdf2Result.Length);
        Buffer.BlockCopy(salt2, 0, ph2Data, salt2.Length + pbkdf2Result.Length, salt2.Length);
        var x = SHA256.HashData(ph2Data);
        
        return x;
    }

    public (long srpId, byte[] B, byte[] bPrivate) GenerateServerSrpSession(byte[] v, int g, byte[] p)
    {
        var srpId = _randomHelper.NextInt64();
        var bPrivate = new byte[256];
        _randomHelper.NextBytes(bPrivate);

        var gBig = new BigInteger(g);
        var pBig = new BigInteger(p, isUnsigned: true, isBigEndian: true);
        var vBig = new BigInteger(v, isUnsigned: true, isBigEndian: true);
        var bBig = new BigInteger(bPrivate, isUnsigned: true, isBigEndian: true);

        // k = H(p | g) - According to SRP protocol
        // g must be padded to 256 bytes (2048 bits) like p, for consistent hashing
        var gPadded = new byte[256];
        var gBigTemp = new BigInteger(g);
        var gBytesTemp = gBigTemp.ToByteArray(isUnsigned: true, isBigEndian: true);
        Buffer.BlockCopy(gBytesTemp, 0, gPadded, 256 - gBytesTemp.Length, gBytesTemp.Length);
        
        var pg = new byte[p.Length + gPadded.Length]; // 512 bytes total
        Buffer.BlockCopy(p, 0, pg, 0, p.Length);
        Buffer.BlockCopy(gPadded, 0, pg, p.Length, gPadded.Length);
        var kHash = SHA256.HashData(pg);
        var kBig = new BigInteger(kHash, isUnsigned: true, isBigEndian: true);

        var kv = BigInteger.Multiply(kBig, vBig);
        var gb = BigInteger.ModPow(gBig, bBig, pBig);
        var B = BigInteger.Add(kv, gb);
        B = BigInteger.Remainder(B, pBig);

        var BBytes = B.ToByteArray(isUnsigned: true, isBigEndian: true);
        return (srpId, BBytes, bPrivate);
    }

    public bool VerifySrpProof(byte[] clientA, byte[] clientM1, byte[] v, byte[] bPrivate, byte[] serverB, int g, byte[] p, byte[] salt1, byte[] salt2)
    {
        try
        {
            _logger.LogInformation("VerifySrpProof - A len: {ALen}, M1 len: {M1Len}, v len: {VLen}, bPrivate len: {BPrivLen}, B len: {BLen}, g: {G}, p len: {PLen}, salt1 len: {Salt1Len}, salt2 len: {Salt2Len}",
                clientA.Length, clientM1.Length, v.Length, bPrivate.Length, serverB.Length, g, p.Length, salt1.Length, salt2.Length);
            
            var ABig = new BigInteger(clientA, isUnsigned: true, isBigEndian: true);
            var pBig = new BigInteger(p, isUnsigned: true, isBigEndian: true);
            
            if (BigInteger.Remainder(ABig, pBig) == BigInteger.Zero)
            {
                _logger.LogWarning("SRP verification failed: A % p == 0");
                return false;
            }

            var vBig = new BigInteger(v, isUnsigned: true, isBigEndian: true);
            var bBig = new BigInteger(bPrivate, isUnsigned: true, isBigEndian: true);

            var AB = new byte[clientA.Length + serverB.Length];
            Buffer.BlockCopy(clientA, 0, AB, 0, clientA.Length);
            Buffer.BlockCopy(serverB, 0, AB, clientA.Length, serverB.Length);
            var uHash = SHA256.HashData(AB);
            var uBig = new BigInteger(uHash, isUnsigned: true, isBigEndian: true);
            
            _logger.LogInformation("Computing u from A||B, u hash len: {UHashLen}", uHash.Length);

            var vu = BigInteger.ModPow(vBig, uBig, pBig);
            var Avu = BigInteger.Multiply(ABig, vu);
            Avu = BigInteger.Remainder(Avu, pBig);
            var S = BigInteger.ModPow(Avu, bBig, pBig);

            var SBytes = S.ToByteArray(isUnsigned: true, isBigEndian: true);
            
            // Pad S to 256 bytes (Telegram requirement)
            var SPadded = new byte[256];
            if (SBytes.Length <= 256)
            {
                Buffer.BlockCopy(SBytes, 0, SPadded, 256 - SBytes.Length, SBytes.Length);
            }
            else
            {
                Buffer.BlockCopy(SBytes, SBytes.Length - 256, SPadded, 0, 256);
            }
            
            var K = SHA256.HashData(SPadded);
            
            _logger.LogInformation("Computed S len: {SLen}, S padded len: {SPaddedLen}, K: {K}", 
                SBytes.Length, SPadded.Length, Convert.ToHexString(K));

            // M1 = H(H(p) XOR H(g) | H(salt1) | H(salt2) | g_a | g_b | k_a)
            // As per https://core.telegram.org/api/srp
            var pHash = SHA256.HashData(p);
            
            // H(g) - g is a small int (2,3,4,5,6,7), convert to big-endian bytes padded to 256
            var gPadded = new byte[256];
            var gBig = new BigInteger(g);
            var gBytes = gBig.ToByteArray(isUnsigned: true, isBigEndian: true);
            Buffer.BlockCopy(gBytes, 0, gPadded, 256 - gBytes.Length, gBytes.Length);
            var gHash = SHA256.HashData(gPadded);
            
            // H(p) XOR H(g)
            var pXorG = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                pXorG[i] = (byte)(pHash[i] ^ gHash[i]);
            }
            
            // For M1 calculation, use ALL bytes of salt1 (including appended 32 random bytes)
            // According to TDLib and official Telegram docs: H(salt1) uses the complete 64-byte value
            // The 64-byte salt1 = [original_32_bytes + random_32_bytes]
            if (salt1.Length == 64)
            {
                _logger.LogInformation("Using ALL 64 bytes of salt1 for M1 calculation (original + random)");
            }
            
            var salt1Hash = SHA256.HashData(salt1);
            var salt2Hash = SHA256.HashData(salt2);
            
            // M1 = H(H(p) XOR H(g) | H(salt1) | H(salt2) | A | B | K)
            // According to Telegram SRP: https://core.telegram.org/api/srp
            var m1Data = new byte[32 + 32 + 32 + clientA.Length + serverB.Length + K.Length];
            int offset = 0;
            Buffer.BlockCopy(pXorG, 0, m1Data, offset, 32);
            offset += 32;
            Buffer.BlockCopy(salt1Hash, 0, m1Data, offset, 32);
            offset += 32;
            Buffer.BlockCopy(salt2Hash, 0, m1Data, offset, 32);
            offset += 32;
            Buffer.BlockCopy(clientA, 0, m1Data, offset, clientA.Length);
            offset += clientA.Length;
            Buffer.BlockCopy(serverB, 0, m1Data, offset, serverB.Length);
            offset += serverB.Length;
            Buffer.BlockCopy(K, 0, m1Data, offset, K.Length);
            
            var expectedM1 = SHA256.HashData(m1Data);
            
            _logger.LogInformation("M1 = H(H(p)⊕H(g) | H(salt1) | H(salt2) | A | B | K), total: {TotalLen}", 
                m1Data.Length);
            
            _logger.LogInformation("Client M1: {ClientM1}, Expected M1: {ExpectedM1}", 
                Convert.ToHexString(clientM1), Convert.ToHexString(expectedM1));
            
            var result = clientM1.SequenceEqual(expectedM1);
            _logger.LogInformation("SRP verification result: {Result}", result);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying SRP proof");
            return false;
        }
    }

    public (int g, byte[] p) GetDefaultSrpParams()
    {
        return (DefaultG, DefaultP);
    }

    public byte[] GenerateSalt(int length = 32)
    {
        var salt = new byte[length];
        _randomHelper.NextBytes(salt);
        return salt;
    }
}
