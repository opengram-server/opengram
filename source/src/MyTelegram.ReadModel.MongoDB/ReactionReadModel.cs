using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using EventFlow.ReadStores;

using EventFlow.MongoDB.ReadStores;

namespace MyTelegram.ReadModel.MongoDB;

[MongoDbCollectionName("reactions")]
[BsonIgnoreExtraElements]
public class ReactionReadModel : IMongoDbReadModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public long? Version { get; set; }
    
    [BsonElement("emoji")]
    public string Emoji { get; set; } = null!;
    
    [BsonElement("title")]
    public string Title { get; set; } = null!;
    
    [BsonElement("premium")]
    public bool Premium { get; set; }
    
    [BsonElement("inactive")]
    public bool Inactive { get; set; }
    
    [BsonElement("static_icon")]
    public long? StaticIcon { get; set; }
    
    [BsonElement("appear_animation")]
    public long? AppearAnimation { get; set; }
    
    [BsonElement("select_animation")]
    public long? SelectAnimation { get; set; }
    
    [BsonElement("activate_animation")]
    public long? ActivateAnimation { get; set; }
    
    [BsonElement("effect_animation")]
    public long? EffectAnimation { get; set; }
    
    [BsonElement("around_animation")]
    public long? AroundAnimation { get; set; }
    
    [BsonElement("center_icon")]
    public long? CenterIcon { get; set; }
}
