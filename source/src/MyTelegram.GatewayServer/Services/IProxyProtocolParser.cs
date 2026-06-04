namespace MyTelegram.GatewayServer.Services;

public interface IProxyProtocolParser
{
    bool IsProxyProtocolV2(in ReadOnlySequence<byte> buffer);
    bool HasEnoughProxyProtocolV2Data(in ReadOnlySequence<byte> data, out int proxyProtocolHeaderLength);
    ProxyProtocolFeature? Parse(ref ReadOnlySequence<byte> buffer);
}