// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// URL with chat statistics
/// See <a href="https://corefork.telegram.org/constructor/StatsURL" />
///</summary>
[JsonDerivedType(typeof(TStatsURL), nameof(TStatsURL))]
public interface IStatsURL : IObject
{
    ///<summary>
    /// Chat statistics
    ///</summary>
    string Url { get; set; }
}
