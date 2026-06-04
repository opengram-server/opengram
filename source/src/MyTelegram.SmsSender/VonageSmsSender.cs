using Microsoft.Extensions.Options;
using Vonage;
using Vonage.Messaging;
using Vonage.Request;

namespace MyTelegram.SmsSender;

public class VonageSmsSender : ISmsSender, ITransientDependency
{
    private readonly IOptionsMonitor<VonageSmsOptions> _options;
    private readonly ILogger<VonageSmsSender> _logger;
    public VonageSmsSender(IOptionsMonitor<VonageSmsOptions> optionsSnapshot, ILogger<VonageSmsSender> logger)
    {
        _options = optionsSnapshot;
        _logger = logger;
    }

    public bool Enabled => _options.CurrentValue.Enabled;

    public async Task SendAsync(SmsMessage smsMessage)
    {
        if (!_options.CurrentValue.Enabled)
        {
            _logger.LogWarning("Vonage sms sender disabled, the code will not be sent. PhoneNumber: {To} Text: {Text}", smsMessage.PhoneNumber, smsMessage.Text);

            return;
        }

        // E.164 format
        var phoneNumber = smsMessage.PhoneNumber;
        if (!phoneNumber.StartsWith("+"))
        {
            phoneNumber = phoneNumber[1..];
        }

        var credentials =
            Credentials.FromApiKeyAndSecret(_options.CurrentValue.ApiKey, _options.CurrentValue.ApiSecret);
        var client = new VonageClient(credentials);
        var response = await client.SmsClient.SendAnSmsAsync(new SendSmsRequest
        {
            To = phoneNumber,
            From = _options.CurrentValue.BrandName,
            Text = smsMessage.Text,
        });

        _logger.LogDebug("Send SMS result: {@Response}", response);

        var message = response.Messages.ElementAtOrDefault(0);
        _logger.LogInformation("Send SMS completed, To={To}, StatusCode={StatusCode}, Status={Status}, MessageId={MessageId}, ErrorText={ErrorText}", message?.To, message?.Status, message?.StatusCode, message?.MessageId, message?.ErrorText);
    }
}