namespace MyTelegram.Converters.Responses.Interfaces;

public interface IChannelResponseConverter
    : IResponseConverter<
        TChannel,
        ILayeredChannel
    >
{
}