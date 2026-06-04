// ReSharper disable All

namespace MyTelegram.Schema.E2e;


[JsonDerivedType(typeof(TBlock), nameof(TBlock))]
public interface IBlock : IObject
{
    ReadOnlyMemory<byte> Signature { get; set; }
    int Flags { get; set; }
    ReadOnlyMemory<byte> PrevBlockHash { get; set; }
    TVector<MyTelegram.Schema.E2e.IChange> Changes { get; set; }
    int Height { get; set; }
    MyTelegram.Schema.E2e.IStateProof StateProof { get; set; }
    ReadOnlyMemory<byte>? SignaturePublicKey { get; set; }
}
