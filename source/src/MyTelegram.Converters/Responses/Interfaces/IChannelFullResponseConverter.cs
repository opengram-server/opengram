namespace MyTelegram.Converters.Responses.Interfaces;

public interface IChannelFullResponseConverter
    : IResponseConverter<
        TChannelFull,
        IChatFull
    >
{
}