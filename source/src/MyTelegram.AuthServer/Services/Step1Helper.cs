namespace MyTelegram.AuthServer.Services;

public class Step1Helper(IFingerprintHelper fingerprintHelper) : IStep1Helper, ISingletonDependency
{
    public Step1Output GetResponse(byte[] nonce)
    {
        var p = AuthConsts.P;
        var q = AuthConsts.Q;

        var serverNonce = RandomNumberGenerator.GetBytes(16);

        var publicKeyFingerprint = fingerprintHelper.GetFingerprint();
        var pq = AuthConsts.Pq;
        var resPq = new TResPQ
        {
            Nonce = nonce,
            ServerNonce = serverNonce,
            Pq = pq,
            ServerPublicKeyFingerprints = new TVector<long> { publicKeyFingerprint }
        };

        return new Step1Output(p, q, serverNonce, resPq);
    }
}