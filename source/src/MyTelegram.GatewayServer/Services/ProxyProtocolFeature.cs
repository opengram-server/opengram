namespace MyTelegram.GatewayServer.Services;

public record ProxyProtocolFeature(
    IPAddress SourceIp,
    IPAddress DestinationIp,
    int SourcePort,
    int DestinationPort);