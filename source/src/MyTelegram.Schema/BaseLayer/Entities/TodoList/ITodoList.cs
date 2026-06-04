// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/TodoList" />
///</summary>
[JsonDerivedType(typeof(TTodoList), nameof(TTodoList))]
public interface ITodoList : IObject
{
    int Flags { get; set; }
    bool OthersCanAppend { get; set; }
    bool OthersCanComplete { get; set; }
    MyTelegram.Schema.ITextWithEntities Title { get; set; }
    TVector<MyTelegram.Schema.ITodoItem> List { get; set; }
}
