// ReSharper disable All

namespace MyTelegram.Schema.Messages;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/messages.BotPreparedInlineMessage" />
///</summary>
[JsonDerivedType(typeof(TBotPreparedInlineMessage), nameof(TBotPreparedInlineMessage))]
public interface IBotPreparedInlineMessage : IObject
{
    ///<summary>
    /// &nbsp;
    ///</summary>
    string Id { get; set; }

    ///<summary>
    /// &nbsp;
    ///</summary>
    int ExpireDate { get; set; }
}
