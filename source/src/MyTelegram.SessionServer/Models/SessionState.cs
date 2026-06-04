using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyTelegram.SessionServer.Models;

public enum SessionStateEnum
{
    Pending,
    Active,
    Suspended,
    Closed,
    Expired
}

public class SessionState
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public long SessionId { get; set; }
    public long UserId { get; set; }
    public long AuthKeyId { get; set; }
    public long Salt { get; set; }
    public int SeqNo { get; set; }
    public long MessageId { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public SessionStateEnum State { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivity { get; set; }
    public DateTime? ClosedAt { get; set; }
    
    // Additional metadata for MTProto connection
    public string Platform { get; set; } = string.Empty;
    public string ClientVersion { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
}
