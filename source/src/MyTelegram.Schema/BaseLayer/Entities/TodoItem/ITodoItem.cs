// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/TodoItem" />
///</summary>
[JsonDerivedType(typeof(TTodoItem), nameof(TTodoItem))]
public interface ITodoItem : IObject
{
    int Id { get; set; }
    MyTelegram.Schema.ITextWithEntities Title { get; set; }
}
