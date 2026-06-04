namespace MyTelegram.AuthServer.Services;

public class FingerprintHelper(IRsaKeyProvider rsaKeyProvider, IMyRsaHelper rsaHelper)
    : IFingerprintHelper,
        ISingletonDependency
{
    private long _fingerprint;

    public long GetFingerprint()
    {
        if (_fingerprint == 0)
        {
            _fingerprint = rsaHelper.GetFingerprintFromPrivateKey(
                rsaKeyProvider.GetRsaPrivateKey()
            );
        }

        return _fingerprint;
    }
}