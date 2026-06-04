namespace MyTelegram;

public class PrivacyValueData(
    PrivacyValueType privacyValueType,
    string? jsonData = null)
{
    public string? JsonData { get; init; } = jsonData;

    public PrivacyValueType PrivacyValueType { get; init; } = privacyValueType;
}