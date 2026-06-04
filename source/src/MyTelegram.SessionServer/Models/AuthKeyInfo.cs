using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyTelegram.SessionServer.Models;

public class AuthKeyInfo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public long AuthKeyId { get; set; }
    public long UserId { get; set; }
    public byte[] KeyData { get; set; } = Array.Empty<byte>();
    public bool IsValid { get; set; }
    public DateTime CreatedAt { get; set; }
}
