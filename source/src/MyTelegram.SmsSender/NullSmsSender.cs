namespace MyTelegram.SmsSender;

public class NullSmsSender(ILogger<NullSmsSender> logger) : INullSmsSender, ITransientDependency
{
    public bool Enabled => false;
    public Task SendAsync(SmsMessage smsMessage)
    {
        logger.LogWarning("NullSMsSender: the code will not be sent.PhoneNumber:{To} Text:{Text}", smsMessage.PhoneNumber, smsMessage.Text);

        return Task.CompletedTask;
    }
}