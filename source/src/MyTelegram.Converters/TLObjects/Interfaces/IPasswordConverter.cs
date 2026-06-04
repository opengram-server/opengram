using MyTelegram.ReadModel;
using MyTelegram.Schema.Account;

namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IPasswordConverter : ILayeredConverter
{
    IPassword ToPassword(IUserPasswordReadModel? userPasswordReadModel);
}