namespace MyTelegram.Domain.Shared;

public record PhoneCallProtocol(
    bool UdpP2p,
    bool UdpReflector,
    int MinLayer,
    int MaxLayer,
    List<string> LibraryVersions);

public record PhoneConnectionInfo(
    long Id,
    string Ip,
    string Ipv6,
    int Port,
    byte[] PeerTag,
    bool IsTurn = false,
    bool IsStun = false,
    string? Username = null,
    string? Password = null);