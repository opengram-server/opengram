namespace MyTelegram.SmsSender;

public class VonageSmsOptions
{
    public bool Enabled { get; set; }
    public string BrandName { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string ApiSecret { get; set; } = null!;
}