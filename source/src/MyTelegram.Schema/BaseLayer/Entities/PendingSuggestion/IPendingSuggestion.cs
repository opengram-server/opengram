// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/PendingSuggestion" />
///</summary>
[JsonDerivedType(typeof(TPendingSuggestion), nameof(TPendingSuggestion))]
public interface IPendingSuggestion : IObject
{
    string Suggestion { get; set; }
    MyTelegram.Schema.ITextWithEntities Title { get; set; }
    MyTelegram.Schema.ITextWithEntities Description { get; set; }
    string Url { get; set; }
}
