// ReSharper disable All

namespace MyTelegram.Schema.Channels;

///<summary>
/// Channel/supergroup participants
/// See <a href="https://corefork.telegram.org/constructor/channels.ChannelParticipants" />
///</summary>
[JsonDerivedType(typeof(TChannelParticipants), nameof(TChannelParticipants))]
[JsonDerivedType(typeof(TChannelParticipantsNotModified), nameof(TChannelParticipantsNotModified))]
[JsonDerivedType(typeof(MyTelegram.Schema.Channels.LayerN.TChannelParticipants), "TChannelParticipantsLayerN")]
public interface IChannelParticipants : IObject
{

}
