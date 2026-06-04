// ReSharper disable All

namespace MyTelegram.Schema.E2e;


[JsonDerivedType(typeof(TGroupState), nameof(TGroupState))]
public interface IGroupState : IObject
{
    TVector<MyTelegram.Schema.E2e.IGroupParticipant> Participants { get; set; }
    int ExternalPermissions { get; set; }
}
