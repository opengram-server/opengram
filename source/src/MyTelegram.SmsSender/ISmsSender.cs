namespace MyTelegram.SmsSender;

public interface ISmsSender
{
    bool Enabled { get; }
    Task SendAsync(SmsMessage smsMessage);
}