using MyTelegram.Domain.Shared;
using MyTelegram.Schema;

namespace MyTelegram.Converters.Extensions;

public static class PhoneCallExtensions
{
    public static TPhoneCallProtocol ToPhoneCallProtocol(this PhoneCallProtocol protocol)
    {
        return new TPhoneCallProtocol
        {
            UdpP2p = protocol.UdpP2p,
            UdpReflector = protocol.UdpReflector,
            MinLayer = protocol.MinLayer,
            MaxLayer = protocol.MaxLayer,
            LibraryVersions = new TVector<string>(protocol.LibraryVersions)
        };
    }

    public static ReadOnlyMemory<byte> ToReadOnlyMemory(this byte[] bytes)
    {
        return new ReadOnlyMemory<byte>(bytes);
    }
}
