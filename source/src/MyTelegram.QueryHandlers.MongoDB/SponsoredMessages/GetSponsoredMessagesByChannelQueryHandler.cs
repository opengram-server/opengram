using MongoDB.Driver;
using MyTelegram.Queries.SponsoredMessages;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB.SponsoredMessages;

public class GetSponsoredMessagesByChannelQueryHandler : IQueryHandler<GetSponsoredMessagesByChannelQuery, IReadOnlyCollection<ISponsoredMessageReadModel>>
{
    private readonly IMongoDatabase _database;

    public GetSponsoredMessagesByChannelQueryHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<IReadOnlyCollection<ISponsoredMessageReadModel>> ExecuteQueryAsync(
        GetSponsoredMessagesByChannelQuery query,
        CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<SponsoredMessageReadModel>("ReadModel-SponsoredMessageReadModel");
        
        var now = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        var filterBuilder = Builders<SponsoredMessageReadModel>.Filter;
        var filter = filterBuilder.Eq(x => x.ChannelId, query.ChannelId);
        
        if (query.OnlyActive)
        {
            filter = filterBuilder.And(
                filter,
                filterBuilder.Eq(x => x.IsActive, true),
                filterBuilder.Or(
                    filterBuilder.Eq(x => x.ExpiresDate, null),
                    filterBuilder.Gt(x => x.ExpiresDate, now)
                )
            );
        }
        
        var messages = await collection.Find(filter)
            .SortBy(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        return messages.Cast<ISponsoredMessageReadModel>().ToList();
    }
}
