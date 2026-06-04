namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IUserConverter : ILayeredConverter
{
    ILayeredUser ToUser(IUserReadModel userReadModel);
    List<ILayeredUser> ToUserList(IEnumerable<IUserReadModel> userReadModels);
}