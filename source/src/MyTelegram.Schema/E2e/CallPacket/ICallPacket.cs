// ReSharper disable All

namespace MyTelegram.Schema.E2e;


[JsonDerivedType(typeof(TCallPacket), nameof(TCallPacket))]
public interface ICallPacket : IObject
{

}
