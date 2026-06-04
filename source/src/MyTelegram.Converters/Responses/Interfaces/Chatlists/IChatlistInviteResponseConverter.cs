using MyTelegram.Schema.Chatlists;

namespace MyTelegram.Converters.Responses.Interfaces.Chatlists;

public interface IChatlistInviteResponseConverter
    : IResponseConverter<
        TChatlistInvite,
        IChatlistInvite
    >
{
}