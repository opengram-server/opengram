// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// Represents a <a href="https://corefork.telegram.org/api/invites#paid-invite-links">Telegram Star subscription »</a>.
/// See <a href="https://corefork.telegram.org/constructor/StarsSubscription" />
///</summary>
[JsonDerivedType(typeof(TStarsSubscription), nameof(TStarsSubscription))]
public interface IStarsSubscription : IObject
{
    ///<summary>
    /// Flags, see <a href="https://corefork.telegram.org/mtproto/TL-combinators#conditional-fields">TL conditional fields</a>
    ///</summary>
    int Flags { get; set; }

    ///<summary>
    /// Whether this subscription was cancelled.
    ///</summary>
    bool Canceled { get; set; }

    ///<summary>
    /// Whether we left the associated private channel, but we can still rejoin it using <a href="https://corefork.telegram.org/method/payments.fulfillStarsSubscription">payments.fulfillStarsSubscription</a> because the current subscription period hasn't expired yet.
    ///</summary>
    bool CanRefulfill { get; set; }

    ///<summary>
    /// Whether this subscription has expired because there are not enough stars on the user's balance to extend it.
    ///</summary>
    bool MissingBalance { get; set; }

    ///<summary>
    /// &nbsp;
    ///</summary>
    bool BotCanceled { get; set; }

    ///<summary>
    /// Subscription ID.
    ///</summary>
    string Id { get; set; }

    ///<summary>
    /// Identifier of the associated private chat.
    /// See <a href="https://corefork.telegram.org/type/Peer" />
    ///</summary>
    MyTelegram.Schema.IPeer Peer { get; set; }

    ///<summary>
    /// Expiration date of the current subscription period.
    ///</summary>
    int UntilDate { get; set; }

    ///<summary>
    /// Pricing of the subscription in Telegram Stars.
    /// See <a href="https://corefork.telegram.org/type/StarsSubscriptionPricing" />
    ///</summary>
    MyTelegram.Schema.IStarsSubscriptionPricing Pricing { get; set; }

    ///<summary>
    /// Invitation link, used to renew the subscription after cancellation or expiration.
    ///</summary>
    string? ChatInviteHash { get; set; }

    ///<summary>
    /// &nbsp;
    ///</summary>
    string? Title { get; set; }

    ///<summary>
    /// &nbsp;
    /// See <a href="https://corefork.telegram.org/type/WebDocument" />
    ///</summary>
    MyTelegram.Schema.IWebDocument? Photo { get; set; }

    ///<summary>
    /// &nbsp;
    ///</summary>
    string? InvoiceSlug { get; set; }
}
