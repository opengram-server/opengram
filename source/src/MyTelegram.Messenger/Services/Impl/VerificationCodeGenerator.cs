namespace MyTelegram.Messenger.Services.Impl;

public class VerificationCodeGenerator(IOptionsMonitor<MyTelegramMessengerServerOptions> options, IRandomHelper randomHelper) : IVerificationCodeGenerator, ITransientDependency
{
    public string Generate()
    {
        var code = options.CurrentValue.FixedVerifyCode;
        if (string.IsNullOrEmpty(code) || string.IsNullOrWhiteSpace(code))
        {
            code = randomHelper.GenerateRandomNumber(options.CurrentValue.VerificationCodeLength);
        }

        return code;
    }
}