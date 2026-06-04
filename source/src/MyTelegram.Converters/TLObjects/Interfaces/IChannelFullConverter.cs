namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IChannelFullConverter : ILayeredConverter
{
    ILayeredChannelFull ToChannelFull(IChannelFullReadModel channelFullReadModel);
}