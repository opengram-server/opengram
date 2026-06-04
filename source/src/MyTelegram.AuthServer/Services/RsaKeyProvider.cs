namespace MyTelegram.AuthServer.Services;

public class RsaKeyProvider(IOptions<MyTelegramAuthServerOptions> options)
    : IRsaKeyProvider,
        ISingletonDependency
{
    private string? _privateKey;

    public string GetRsaPrivateKey()
    {
        if (!string.IsNullOrEmpty(_privateKey))
        {
            return _privateKey;
        }

        if (!File.Exists(options.Value.PrivateKeyFilePath))
        {
            throw new FileNotFoundException(
                "Private key not exists",
                options.Value.PrivateKeyFilePath
            );
        }

        _privateKey = File.ReadAllText(options.Value.PrivateKeyFilePath);

        if (string.IsNullOrEmpty(_privateKey))
        {
            throw new InvalidOperationException("Private key can not be null");
        }

        return _privateKey;
    }
}