namespace MyTelegram.ReadModel.Interfaces;

public interface ILanguageReadModel : IReadModel
{
    DeviceType Platform { get; }
    string Name { get; }
    string NativeName { get; }
    string LanguageCode { get; }
    string PluralCode { get; }
    string TranslationsUrl { get; }
    bool IsEnabled { get; }
    int TranslatedCount { get; }

    int LanguageVersion { get; }
}