// ReSharper disable All

namespace MyTelegram.Schema.E2e;


[JsonDerivedType(typeof(TChangeNoop), "TChangeNoopLayer0")]
[JsonDerivedType(typeof(TChangeSetValue), "TChangeSetValueLayer0")]
[JsonDerivedType(typeof(TChangeSetGroupState), "TChangeSetGroupStateLayer0")]
[JsonDerivedType(typeof(TChangeSetSharedKey), "TChangeSetSharedKeyLayer0")]
public interface IChange : IObject
{

}
