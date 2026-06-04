namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IChannelConverter : ILayeredConverter
{
    ILayeredChannel ToChannel(IChannelReadModel channelReadModel);
}