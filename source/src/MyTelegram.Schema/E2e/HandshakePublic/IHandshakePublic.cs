// ReSharper disable All

namespace MyTelegram.Schema.E2e;


[JsonDerivedType(typeof(THandshakeQR), "THandshakeQRLayer0")]
[JsonDerivedType(typeof(THandshakeLoginExport), "THandshakeLoginExportLayer0")]
public interface IHandshakePublic : IObject
{

}
