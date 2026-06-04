// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// Contains info about a <a href="https://corefork.telegram.org/api/giveaways">prepaid giveaway »</a>.
/// See <a href="https://corefork.telegram.org/constructor/PrepaidGiveaway" />
///</summary>
[JsonDerivedType(typeof(TPrepaidGiveaway), nameof(TPrepaidGiveaway))]
[JsonDerivedType(typeof(TPrepaidStarsGiveaway), nameof(TPrepaidStarsGiveaway))]
public interface IPrepaidGiveaway : IObject
{
    ///<summary>
    /// Prepaid giveaway ID.
    ///</summary>
    long Id { get; set; }

    ///<summary>
    /// Number of giveaway winners
    ///</summary>
    int Quantity { get; set; }

    ///<summary>
    /// When was the giveaway paid for
    ///</summary>
    int Date { get; set; }
}
