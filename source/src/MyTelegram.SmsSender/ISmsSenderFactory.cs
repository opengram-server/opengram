namespace MyTelegram.SmsSender;

public interface ISmsSenderFactory
{
    ISmsSender Create(string phoneNumber);
}