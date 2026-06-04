using System.Security.Cryptography;

namespace MyTelegram.Messenger.Services;

public interface IIceCredentialsService
{
    (string ufrag, string pwd) Generate();
}

public class IceCredentialsService : IIceCredentialsService
{
    public (string ufrag, string pwd) Generate()
    {
        var ufragBytes = RandomNumberGenerator.GetBytes(12);
        var pwdBytes = RandomNumberGenerator.GetBytes(24);

        var ufrag = ToBase64Url(ufragBytes).Substring(0, 16);
        var pwd = ToBase64Url(pwdBytes).Substring(0, 32);

        return (ufrag, pwd);
    }

    private static string ToBase64Url(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
