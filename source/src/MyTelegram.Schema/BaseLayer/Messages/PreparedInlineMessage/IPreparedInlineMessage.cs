// ReSharper disable All

namespace MyTelegram.Schema.Messages;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/messages.PreparedInlineMessage" />
///</summary>
[JsonDerivedType(typeof(TPreparedInlineMessage), nameof(TPreparedInlineMessage))]
public interface IPreparedInlineMessage : IObject
{
    ///<summary>
    /// &nbsp;
    ///</summary>
    long QueryId { get; set; }

    ///<summary>
    /// &nbsp;
    /// See <a href="https://corefork.telegram.org/type/BotInlineResult" />
    ///</summary>
    MyTelegram.Schema.IBotInlineResult Result { get; set; }

    ///<summary>
    /// &nbsp;
    /// See <a href="https://corefork.telegram.org/type/InlineQueryPeerType" />
    ///</summary>
    TVector<MyTelegram.Schema.IInlineQueryPeerType> PeerTypes { get; set; }

    ///<summary>
    /// &nbsp;
    ///</summary>
    int CacheTime { get; set; }

    ///<summary>
    /// &nbsp;
    /// See <a href="https://corefork.telegram.org/type/User" />
    ///</summary>
    TVector<MyTelegram.Schema.IUser> Users { get; set; }
}
