namespace MyTelegram.ReadModel.Interfaces;

public interface ISponsoredMessageReadModel : IReadModel
{
    string Id { get; }
    long ChannelId { get; }
    string Title { get; }
    string Message { get; }
    string Url { get; }
    string ButtonText { get; }
    string? PhotoUrl { get; }
    string? SponsorInfo { get; }
    string? AdditionalInfo { get; }
    bool IsActive { get; }
    bool Recommended { get; }
    bool CanReport { get; }
    int? PostsBetween { get; }
    int CreatedDate { get; }
    int? ExpiresDate { get; }
    int DisplayCount { get; }
    int ClickCount { get; }
}
