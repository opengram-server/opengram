namespace MyTelegram.DataSeeder.DataSeeders;

public interface IUserDataSeeder
{
    Task SeedAsync();

    Task<bool> CreateUserIfNeedAsync(long userId,
        string phoneNumber,
        string firstName,
        string? lastName,
        string? userName,
        bool bot);
}