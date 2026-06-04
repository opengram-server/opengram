namespace MyTelegram.Messenger.Services.Interfaces;

public interface ICountryHelper
{
    bool TryGetCountryCodeItem(string countryCode, [NotNullWhen(true)] out CountryCodeItem? countryCodeItem);
    IReadOnlyCollection<CountryItem> GetAllCountryList();
    void InitAllCountries();
    string GetCountryCodeByPhoneNumber(string phoneNumber);
    string? GetCountryIso2ByPhoneNumber(string phoneNumber);
}


public record CountryItem(bool Hidden, string Iso2, string DefaultName, string? Name, List<CountryCodeItem> CountryCodes);
public record CountryCodeItem(string CountryCode, List<string>? Prefixes, List<string>? Patterns, List<int>? PhoneNumberLengths);