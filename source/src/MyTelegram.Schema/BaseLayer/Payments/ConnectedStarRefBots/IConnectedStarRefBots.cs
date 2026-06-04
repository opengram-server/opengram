// ReSharper disable All

namespace MyTelegram.Schema.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/payments.ConnectedStarRefBots" />
///</summary>
[JsonDerivedType(typeof(TConnectedStarRefBots), nameof(TConnectedStarRefBots))]
public interface IConnectedStarRefBots : IObject
{
    ///<summary>
    /// &nbsp;
    ///</summary>
    int Count { get; set; }

    ///<summary>
    /// &nbsp;
    /// See <a href="https://corefork.telegram.org/type/ConnectedBotStarRef" />
    ///</summary>
    TVector<MyTelegram.Schema.IConnectedBotStarRef> ConnectedBots { get; set; }

    ///<summary>
    /// &nbsp;
    /// See <a href="https://corefork.telegram.org/type/User" />
    ///</summary>
    TVector<MyTelegram.Schema.IUser> Users { get; set; }
}
