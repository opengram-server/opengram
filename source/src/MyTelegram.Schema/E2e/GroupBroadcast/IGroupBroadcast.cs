// ReSharper disable All

namespace MyTelegram.Schema.E2e;


[JsonDerivedType(typeof(TGroupBroadcastNonceCommit), "TGroupBroadcastNonceCommitLayer0")]
[JsonDerivedType(typeof(TGroupBroadcastNonceReveal), "TGroupBroadcastNonceRevealLayer0")]
public interface IGroupBroadcast : IObject
{
    ReadOnlyMemory<byte> Signature { get; set; }
    long UserId { get; set; }
    int ChainHeight { get; set; }
    ReadOnlyMemory<byte> ChainHash { get; set; }
}
