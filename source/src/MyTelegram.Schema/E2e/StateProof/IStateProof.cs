// ReSharper disable All

namespace MyTelegram.Schema.E2e;


[JsonDerivedType(typeof(TStateProof), nameof(TStateProof))]
public interface IStateProof : IObject
{
    int Flags { get; set; }
    ReadOnlyMemory<byte> KvHash { get; set; }
    MyTelegram.Schema.E2e.IGroupState? GroupState { get; set; }
    MyTelegram.Schema.E2e.ISharedKey? SharedKey { get; set; }
}
