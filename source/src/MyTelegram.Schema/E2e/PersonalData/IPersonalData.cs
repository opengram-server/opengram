// ReSharper disable All

namespace MyTelegram.Schema.E2e;


[JsonDerivedType(typeof(TPersonalData), nameof(TPersonalData))]
public interface IPersonalData : IObject
{
    ReadOnlyMemory<byte> PublicKey { get; set; }
    TVector<MyTelegram.Schema.E2e.IPersonalOnServer> Data { get; set; }
}
