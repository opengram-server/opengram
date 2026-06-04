namespace MyTelegram.Domain.Aggregates.QrCode;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<QrCodeId>))]
public class QrCodeId(string value) : Identity<QrCodeId>(value)
{
    public static QrCodeId Create(string token)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"qrcode-{token}");
    }
}
