namespace MyTelegram.DataSeeder;

public class MyTelegramDataSeederOptions
{
    public bool UploadNewDocumentFiles { get; set; }
    public MyTelegramBotOptions MyTelegramBotOptions { get; set; } = default!;
    public bool CreateTestUsers { get; set; }
}