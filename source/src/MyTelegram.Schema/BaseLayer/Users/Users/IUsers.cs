// ReSharper disable All

namespace MyTelegram.Schema.Users;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/users.Users" />
///</summary>
[JsonDerivedType(typeof(TUsers), nameof(TUsers))]
[JsonDerivedType(typeof(TUsersSlice), nameof(TUsersSlice))]
public interface IUsers : IObject
{
    TVector<MyTelegram.Schema.IUser> Users { get; set; }
}
