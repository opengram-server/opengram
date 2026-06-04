namespace MyTelegram.Services;

public interface ISrpService
{
    byte[] ComputeVerifier(string password, byte[] salt1, byte[] salt2, int g, byte[] p);
    byte[] ComputeX(string password, byte[] salt1, byte[] salt2);
    (long srpId, byte[] B, byte[] bPrivate) GenerateServerSrpSession(byte[] v, int g, byte[] p);
    bool VerifySrpProof(byte[] clientA, byte[] clientM1, byte[] v, byte[] bPrivate, byte[] serverB, int g, byte[] p, byte[] salt1, byte[] salt2);
    (int g, byte[] p) GetDefaultSrpParams();
    byte[] GenerateSalt(int length = 32);
}
