namespace MyTelegram.Converters.Responses.Interfaces;

public interface IUserResponseConverter
    : IResponseConverter<
        TUser,
        ILayeredUser
    >
{
}