namespace MyTelegram.SmsSender;

public class SmsMessage(string phoneNumber, string text)
{
    public string PhoneNumber { get; } = phoneNumber;

    public string Text { get; } = text;

    public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();
}