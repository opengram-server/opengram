using MongoDB.Driver;

namespace MyTelegram.QueryHandlers.MongoDB.Reactions;

public class GetAvailableReactionsQueryHandler : IQueryHandler<GetAvailableReactionsQuery, IReadOnlyCollection<IReactionReadModel>>
{
    private readonly IMongoDatabase _database;

    public GetAvailableReactionsQueryHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<IReadOnlyCollection<IReactionReadModel>> ExecuteQueryAsync(GetAvailableReactionsQuery query, CancellationToken cancellationToken)
    {
        // Load reactions from MongoDB reactions collection (created by admin panel)
        var collection = _database.GetCollection<ReactionReadModel>("reactions");
        var reactions = await collection.Find(_ => true).ToListAsync(cancellationToken);
        return reactions;
    }
}

// ReadModel for reactions collection
public class ReactionReadModel : IReactionReadModel
{
    [global::MongoDB.Bson.Serialization.Attributes.BsonId]
    [global::MongoDB.Bson.Serialization.Attributes.BsonRepresentation(global::MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public long? Version { get; set; }
    
    [global::MongoDB.Bson.Serialization.Attributes.BsonElement("emoji")]
    public string Emoji { get; set; } = null!;
    
    [global::MongoDB.Bson.Serialization.Attributes.BsonElement("title")]
    public string Title { get; set; } = null!;
    
    [global::MongoDB.Bson.Serialization.Attributes.BsonElement("premium")]
    public bool Premium { get; set; }
    
    [global::MongoDB.Bson.Serialization.Attributes.BsonElement("inactive")]
    public bool Inactive { get; set; }
    
    [global::MongoDB.Bson.Serialization.Attributes.BsonElement("static_icon")]
    public long? StaticIconDocumentId { get; set; }
    
    [global::MongoDB.Bson.Serialization.Attributes.BsonElement("appear_animation")]
    public long? AppearAnimationDocumentId { get; set; }
    
    [global::MongoDB.Bson.Serialization.Attributes.BsonElement("select_animation")]
    public long? SelectAnimationDocumentId { get; set; }
    
    [global::MongoDB.Bson.Serialization.Attributes.BsonElement("activate_animation")]
    public long? ActivateAnimationDocumentId { get; set; }
    
    [global::MongoDB.Bson.Serialization.Attributes.BsonElement("effect_animation")]
    public long? EffectAnimationDocumentId { get; set; }
    
    [global::MongoDB.Bson.Serialization.Attributes.BsonElement("around_animation")]
    public long? AroundAnimationDocumentId { get; set; }
    
    [global::MongoDB.Bson.Serialization.Attributes.BsonElement("center_icon")]
    public long? CenterIconDocumentId { get; set; }
}
