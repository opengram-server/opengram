namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IMessageFwdHeaderConverter : ILayeredConverter
{
    IMessageFwdHeader? ToMessageFwdHeader(MessageFwdHeader? fwdHeader);
}