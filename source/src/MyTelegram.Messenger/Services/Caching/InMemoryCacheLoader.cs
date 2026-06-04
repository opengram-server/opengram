namespace MyTelegram.Messenger.Services.Caching;

public class InMemoryCacheLoader(IQueryProcessor queryProcessor,
    IContactCacheAppService contactCacheAppService,
    IOptionsMonitor<MyTelegramMessengerServerOptions> options,
    ILogger<InMemoryCacheLoader> logger) : IInMemoryCacheLoader, ITransientDependency
{
    private const int PageSize = 10000;

    public async Task LoadAsync()
    {
        await LoadContactsAsync();
    }

    private async Task LoadContactsAsync()
    {
        var totalCount = await queryProcessor.ProcessAsync(new GetTotalContactCountQuery());
        if (totalCount < options.CurrentValue.MaxInMemoryContactCount)
        {
            var count = 0;

            if (totalCount > 0)
            {
                var hasMoreData = true;
                var skip = 0;
                while (hasMoreData)
                {
                    var contactReadModels = await queryProcessor.ProcessAsync(new GetAllContactsQuery(skip, PageSize));
                    hasMoreData = contactReadModels.Count == PageSize;
                    count += contactReadModels.Count;
                    foreach (var contactReadModel in contactReadModels)
                    {
                        contactCacheAppService.Add(contactReadModel.SelfUserId, contactReadModel.TargetUserId);
                    }

                    skip += PageSize;
                }
            }

            logger.LogInformation("Loaded {ContactCount} contacts into memory", count);
        }
        else
        {
            logger.LogInformation("The total count of contacts exceeds {MaxCount} and does not need to be loaded into memory", options.CurrentValue.MaxInMemoryContactCount);
        }
    }
}