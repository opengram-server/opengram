namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IMessageServiceConverter : ILayeredConverter
{
    ILayeredServiceMessage ToMessage(MessageItem messageItem);
    ILayeredServiceMessage ToMessage(IMessageReadModel messageReadModel);
}