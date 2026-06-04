using System.Buffers.Binary;
using System.Net.Sockets;

namespace MyTelegram.GatewayServer.Services;

// https://github.com/aspnet/AspLabs/blob/main/src/ProxyProtocol/ProxyProtocol.Sample/ProxyProtocol.cs
// https://github.com/OnKey/ProxyProtocol/blob/main/ProxyProtocol/ProxyProtocol.cs
public class ProxyProtocolParser : IProxyProtocolParser, ITransientDependency
{
    private const int Ipv4Length = 4;
    private const int Ipv6Length = 16;
    private const int SignatureLength = 16;

    // The proxy protocol marker.
    private static ReadOnlySpan<byte> Preamble =>
        [0x0D, 0x0A, 0x0D, 0x0A, 0x00, 0x0D, 0x0A, 0x51, 0x55, 0x49, 0x54, 0x0A];

    public bool HasEnoughProxyProtocolV2Data(in ReadOnlySequence<byte> buffer, out int proxyHeaderLength)
    {
        proxyHeaderLength = 0;
        if (buffer.Length < SignatureLength)
        {
            return false;
        }

        Span<byte> lengthSpan = stackalloc byte[2];
        buffer.Slice(14, 2).CopyTo(lengthSpan);

        proxyHeaderLength = BinaryPrimitives.ReadInt16BigEndian(lengthSpan) + SignatureLength;

        return buffer.Length >= proxyHeaderLength;
    }

    public bool IsProxyProtocolV2(in ReadOnlySequence<byte> buffer)
    {
        if (buffer.Length >= Preamble.Length)
        {
            Span<byte> prefixSpan = stackalloc byte[Preamble.Length];
            buffer.Slice(0, prefixSpan.Length).CopyTo(prefixSpan);
            if (prefixSpan.SequenceEqual(Preamble))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Proxy Protocol v2: https://www.haproxy.org/download/1.8/doc/proxy-protocol.txt Section 2.2
    ///     Preamble(12 bytes) : 0D-0A-0D-0A-00-0D-0A-51-55-49-54-0A
    ///     -21                        Version + stream    12
    ///     -11                        TCP over IPv4       13
    ///     -00-14                     length              14
    ///     -AC-1C-00-04               src address         16
    ///     -01-02-03-04               dest address        20
    ///     -D7-9A                     src port            24
    ///     -13-88                     dest port           26
    ///     -EE                        PP2_TYPE_AZURE      28
    ///     -00-05                     length              29
    ///     -01                        LINKID type         31
    ///     -33-00-00-26               LINKID              32.
    /// </summary>
    public ProxyProtocolFeature? Parse(ref ReadOnlySequence<byte> buffer)
    {
        if (!HasEnoughProxyProtocolV2Data(buffer, out var proxyProtocolHeaderLength))
        {
            return null;
        }

        if (!IsProxyProtocolV2(buffer))
        {
            return null;
        }

        return ExtractProxyProtocolV2Data(ref buffer, proxyProtocolHeaderLength);
    }

    private static ProxyProtocolFeature ExtractProxyProtocolV2Data(ref ReadOnlySequence<byte> buffer,
        int proxyHeaderLength)
    {
        Span<byte> span = new byte[proxyHeaderLength];
        buffer.Slice(0, span.Length).CopyTo(span);
        var isProxyProtocolV2 = (span[12] & 0xF0) == 0x20;
        if (!isProxyProtocolV2)
        {
            throw new ArgumentException("Only support proxy protocol v2");
        }

        var isProxy = (span[12] & 0x0F) == 0x01;
        if (!isProxy)
        {
            throw new ArgumentException("Only support proxy command");
        }

        var addressFamily = GetAddressFamily(span);
        ProxyProtocolFeature feature;
        switch (addressFamily)
        {
            case AddressFamily.InterNetwork:
                feature = new ProxyProtocolFeature(GetSourceAddressIpv4(span),
                    GetDestinationAddressIpv4(span),
                    GetSourcePortIpv4(span),
                    GetDestinationPortIpv4(span)
                );
                break;

            case AddressFamily.InterNetworkV6:
                feature = new ProxyProtocolFeature(GetSourceAddressIpv6(span),
                    GetDestinationAddressIpv6(span),
                    GetSourcePortIpv6(span),
                    GetDestinationPortIpv6(span)
                );
                break;

            default:
                throw new NotSupportedException();
        }

        buffer = buffer.Slice(proxyHeaderLength);

        return feature;
    }

    private static AddressFamily GetAddressFamily(ReadOnlySpan<byte> header)
    {
        var family = header[13] & 0xF0;
        return family switch
        {
            0x00 => AddressFamily.Unspecified,
            0x10 => AddressFamily.InterNetwork,
            0x20 => AddressFamily.InterNetworkV6,
            0x30 => AddressFamily.Unix,
            _ => throw new ArgumentException("Invalid address family")
        };
    }

    private static IPAddress GetDestinationAddressIpv4(ReadOnlySpan<byte> span)
    {
        return new IPAddress(span.Slice(SignatureLength + Ipv4Length, Ipv4Length));
    }

    private static IPAddress GetDestinationAddressIpv6(ReadOnlySpan<byte> span)
    {
        return new IPAddress(span.Slice(SignatureLength + Ipv6Length, Ipv6Length));
    }

    private static int GetDestinationPortIpv4(ReadOnlySpan<byte> span)
    {
        return BinaryPrimitives.ReadInt16BigEndian(span.Slice(SignatureLength + 2 * Ipv4Length + 2, 2));
    }

    private static int GetDestinationPortIpv6(ReadOnlySpan<byte> span)
    {
        return BinaryPrimitives.ReadInt16BigEndian(span.Slice(SignatureLength + 2 * Ipv6Length + 2, 2));
    }

    private static IPAddress GetSourceAddressIpv4(ReadOnlySpan<byte> span)
    {
        return new IPAddress(span.Slice(SignatureLength, Ipv4Length));
    }

    private static IPAddress GetSourceAddressIpv6(ReadOnlySpan<byte> span)
    {
        return new IPAddress(span.Slice(SignatureLength, Ipv6Length));
    }

    private static int GetSourcePortIpv4(ReadOnlySpan<byte> span)
    {
        return BinaryPrimitives.ReadInt16BigEndian(span.Slice(SignatureLength + 2 * Ipv4Length, 2));
    }

    private static int GetSourcePortIpv6(ReadOnlySpan<byte> span)
    {
        return BinaryPrimitives.ReadInt16BigEndian(span.Slice(SignatureLength + 2 * Ipv6Length, 2));
    }
}