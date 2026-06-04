// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/SponsoredPeer" />
///</summary>
[JsonDerivedType(typeof(TSponsoredPeer), nameof(TSponsoredPeer))]
public interface ISponsoredPeer : IObject
{
    int Flags { get; set; }
    ReadOnlyMemory<byte> RandomId { get; set; }
    MyTelegram.Schema.IPeer Peer { get; set; }
    string? SponsorInfo { get; set; }
    string? AdditionalInfo { get; set; }
}
