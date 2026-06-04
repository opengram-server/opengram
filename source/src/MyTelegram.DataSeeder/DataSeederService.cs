namespace MyTelegram.DataSeeder;

public class DataSeederService(
    ILogger<DataSeederService> logger,
    IDataSeederHelper dataSeederHelper,
    IUserDataSeeder userDataSeeder,
	IEnumerable<IDataSeeder> dataSeeders
) : IDataSeederService, ITransientDependency
{
    public async Task SeedAllAsync()
    {
        try
        {
            var config = await dataSeederHelper.LoadDataSeederConfigAsync();

            // Users
            if (!config.IsUserCreated)
            {
                await userDataSeeder.SeedAsync();
                config.IsUserCreated = true;
            }
			
			foreach (var dataSeeder in dataSeeders)
            {
                await dataSeeder.SeedAsync();
            }
        }
        finally
        {
            await dataSeederHelper.SaveDataSeederConfigAsync();
        }

        logger.LogInformation("All data created");
    }
}