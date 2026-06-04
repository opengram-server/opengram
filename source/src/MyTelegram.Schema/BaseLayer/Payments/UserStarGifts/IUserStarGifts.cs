// ReSharper disable All

namespace MyTelegram.Schema.Payments;

/// <summary>
/// User's received star gifts
/// See <a href="https://corefork.telegram.org/constructor/payments.UserStarGifts" />
/// </summary>
[JsonDerivedType(typeof(TUserStarGifts), nameof(TUserStarGifts))]
public interface IUserStarGifts : IObject
{

}
