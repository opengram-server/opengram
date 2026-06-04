// ReSharper disable All

namespace MyTelegram.Schema.E2e;


[JsonDerivedType(typeof(TPersonalOnClient), nameof(TPersonalOnClient))]
public interface IPersonalOnClient : IObject
{
    int SignedAt { get; set; }
    MyTelegram.Schema.E2e.IPersonal Personal { get; set; }
}
