// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/SuggestedPost" />
///</summary>
[JsonDerivedType(typeof(TSuggestedPost), nameof(TSuggestedPost))]
public interface ISuggestedPost : IObject
{
    int Flags { get; set; }
    bool Accepted { get; set; }
    bool Rejected { get; set; }
    MyTelegram.Schema.IStarsAmount? Price { get; set; }
    int? ScheduleDate { get; set; }
}
