namespace MyTelegram.DataSeeder.DataSeeders;

public interface IDataSeederHelper
{
    Task<DataSeederConfig> LoadDataSeederConfigAsync();
    Task SaveDataSeederConfigAsync();
    Task<string?> GetJsonTextAsync(string jsonFileName);
    Task<T?> ReadDataFromFileAsync<T>(string jsonFileName);
    Task ResetDataSeederConfigAsync();
}