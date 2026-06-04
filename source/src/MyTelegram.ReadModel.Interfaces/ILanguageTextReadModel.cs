namespace MyTelegram.ReadModel.Interfaces;

public interface ILanguageTextReadModel : IReadModel
{
    DeviceType Platform { get; }
    string LanguageCode { get; }
    string Key { get; }
    string Value { get; }
    int LanguageVersion { get; }
}