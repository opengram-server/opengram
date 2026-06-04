using MongoDB.Driver;
using MyTelegram.Queries.SponsoredMessages;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB.SponsoredMessages;

public class GetAllSponsoredMessagesQueryHandler : IQueryHandler<GetAllSponsoredMessagesQuery, IReadOnlyCollection<ISponsoredMessageReadModel>>
{
    private readonly IMongoDatabase _database;

    public GetAllSponsoredMessagesQueryHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<IReadOnlyCollection<ISponsoredMessageReadModel>> ExecuteQueryAsync(
        GetAllSponsoredMessagesQuery query,
        CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<SponsoredMessageReadModel>("ReadModel-SponsoredMessageReadModel");
        
        var filterBuilder = Builders<SponsoredMessageReadModel>.Filter;
        var filters = new List<FilterDefinition<SponsoredMessageReadModel>>();
        
        if (query.IsActive.HasValue)
        {
            filters.Add(filterBuilder.Eq(x => x.IsActive, query.IsActive.Value));
        }
        
        if (query.ChannelId.HasValue)
        {
            filters.Add(filterBuilder.Eq(x => x.ChannelId, query.ChannelId.Value));
        }
        
        var filter = filters.Any() 
            ? filterBuilder.And(filters) 
            : filterBuilder.Empty;
        
        var messages = await collection.Find(filter)
            .SortByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        return messages.Cast<ISponsoredMessageReadModel>().ToList();
    }
}
