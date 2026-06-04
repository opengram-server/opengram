// ReSharper disable All

namespace MyTelegram.Schema.Account;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/account.PaidMessagesRevenue" />
///</summary>
[JsonDerivedType(typeof(TPaidMessagesRevenue), nameof(TPaidMessagesRevenue))]
public interface IPaidMessagesRevenue : IObject
{
    long StarsAmount { get; set; }
}
