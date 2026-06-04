namespace MyTelegram.GatewayServer;

public static class CertificateHelper
{
    public static X509Certificate2? CreateX509Certificate(GatewayServerItem serverItem)
    {
        if (serverItem.Ssl)
        {
            if (string.IsNullOrEmpty(serverItem.CertPemFilePath) ||
                string.IsNullOrEmpty(serverItem.KeyPemFilePath))
            {
                throw new ArgumentException("CertPemFilePath or KeyPemFilePath is null");
            }

            var cert = X509Certificate2.CreateFromPemFile(serverItem.CertPemFilePath, serverItem.KeyPemFilePath);
            // https://github.com/dotnet/runtime/issues/23749
            // https://github.com/dotnet/runtime/issues/27493
            if (OperatingSystem.IsWindows())
            {
#pragma warning disable SYSLIB0057
                return new X509Certificate2(cert.Export(X509ContentType.Pfx));
#pragma warning restore SYSLIB0057
            }

            return cert;
        }

        return null;
    }
}