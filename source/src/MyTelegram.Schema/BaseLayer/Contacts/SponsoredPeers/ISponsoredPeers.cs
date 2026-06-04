// ReSharper disable All

namespace MyTelegram.Schema.Contacts;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/contacts.SponsoredPeers" />
///</summary>
[JsonDerivedType(typeof(TSponsoredPeersEmpty), nameof(TSponsoredPeersEmpty))]
[JsonDerivedType(typeof(TSponsoredPeers), nameof(TSponsoredPeers))]
public interface ISponsoredPeers : IObject
{

}
