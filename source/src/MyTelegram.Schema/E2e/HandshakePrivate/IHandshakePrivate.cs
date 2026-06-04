// ReSharper disable All

namespace MyTelegram.Schema.E2e;


[JsonDerivedType(typeof(THandshakePrivateAccept), "THandshakePrivateAcceptLayer0")]
[JsonDerivedType(typeof(THandshakePrivateFinish), "THandshakePrivateFinishLayer0")]
public interface IHandshakePrivate : IObject
{
    ReadOnlyMemory<byte> AlicePK { get; set; }
    ReadOnlyMemory<byte> BobPK { get; set; }
    long AliceUserId { get; set; }
    long BobUserId { get; set; }
    ReadOnlyMemory<byte> AliceNonce { get; set; }
    ReadOnlyMemory<byte> BobNonce { get; set; }
}
