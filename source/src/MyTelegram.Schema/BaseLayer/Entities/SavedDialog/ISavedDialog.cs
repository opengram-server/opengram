// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// Represents a <a href="https://corefork.telegram.org/api/saved-messages">saved message dialog »</a>.
/// See <a href="https://corefork.telegram.org/constructor/SavedDialog" />
///</summary>
[JsonDerivedType(typeof(TSavedDialog), nameof(TSavedDialog))]
[JsonDerivedType(typeof(TMonoForumDialog), nameof(TMonoForumDialog))]
public interface ISavedDialog : IObject
{
    int Flags { get; set; }
    MyTelegram.Schema.IPeer Peer { get; set; }

    ///<summary>
    /// The latest message ID
    ///</summary>
    int TopMessage { get; set; }
}
