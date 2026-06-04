// ReSharper disable All

namespace MyTelegram.Schema.E2e;


[JsonDerivedType(typeof(TGroupParticipant), nameof(TGroupParticipant))]
public interface IGroupParticipant : IObject
{
    long UserId { get; set; }
    ReadOnlyMemory<byte> PublicKey { get; set; }
    int Flags { get; set; }
    bool AddUsers { get; set; }
    bool RemoveUsers { get; set; }
    int Version { get; set; }
}
