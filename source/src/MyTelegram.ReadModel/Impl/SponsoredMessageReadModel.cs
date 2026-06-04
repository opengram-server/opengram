using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyTelegram.ReadModel.Impl;

public class SponsoredMessageReadModel : ISponsoredMessageReadModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    
    public long ChannelId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string ButtonText { get; set; } = null!;
    public string? PhotoUrl { get; set; }
    public string? SponsorInfo { get; set; }
    public string? AdditionalInfo { get; set; }
    public bool IsActive { get; set; }
    public bool Recommended { get; set; }
    public bool CanReport { get; set; } = true;
    public int? PostsBetween { get; set; }
    public int CreatedDate { get; set; }
    public int? ExpiresDate { get; set; }
    public int DisplayCount { get; set; }
    public int ClickCount { get; set; }
    public long? Version { get; set; }
}
