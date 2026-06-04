// ReSharper disable All

namespace MyTelegram.Schema.E2e;


[JsonDerivedType(typeof(TPersonalOnServer), nameof(TPersonalOnServer))]
public interface IPersonalOnServer : IObject
{
    ReadOnlyMemory<byte> Signature { get; set; }
    int SignedAt { get; set; }
    MyTelegram.Schema.E2e.IPersonal Personal { get; set; }
}
