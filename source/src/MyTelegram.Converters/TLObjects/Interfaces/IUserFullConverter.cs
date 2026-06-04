namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IUserFullConverter : ILayeredConverter
{
    IUserFull ToUserFull(IUserReadModel userReadModel);
}