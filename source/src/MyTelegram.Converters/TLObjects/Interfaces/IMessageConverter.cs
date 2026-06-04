namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IMessageConverter : ILayeredConverter
{
    ILayeredMessage ToMessage(IMessageReadModel messageReadModel);
    ILayeredMessage ToMessage(MessageItem messageItem);
}