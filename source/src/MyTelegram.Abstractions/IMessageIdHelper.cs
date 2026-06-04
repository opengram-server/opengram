namespace MyTelegram.Abstractions;

public interface IMessageIdHelper
{
    long GenerateMessageId();

    long GenerateUniqueId();
}