namespace MyTelegram.DataSeeder.DataSeeders;

public class DataSeeder<TData>(
    ILogger<DataSeeder<TData>> logger,
    IDataSeederHelper dataSeederHelper) : IDataSeeder<TData>
{
    public async Task SeedAsync(string jsonFileName, Func<TData, Task> seedAction)
    {
        var datas = await dataSeederHelper.ReadDataFromFileAsync<List<TData>>(jsonFileName);
        if (datas?.Count > 0)
        {
            foreach (var data in datas)
            {
                await seedAction(data);
            }

            logger.LogInformation("{TypeName} created successfully, count: {Count}", typeof(TData).Name, datas.Count);
        }
    }
}