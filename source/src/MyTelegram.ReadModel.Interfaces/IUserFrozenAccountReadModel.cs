using MyTelegram.Domain;

namespace MyTelegram.ReadModel.Interfaces;

public interface IUserFrozenAccountReadModel : IReadModel
{
    string Id { get; }
    long UserId { get; }
    
    // Freeze dates
    int FreezeSinceDate { get; }
    int FreezeUntilDate { get; }
    
    // Reason and status
    FreezeReason Reason { get; }
    FreezeStatus Status { get; }
    
    // Appeal
    string? AppealUrl { get; }
    DateTime? AppealSubmittedDate { get; }
    string? AppealText { get; }
    
    // Moderator info
    long? ModeratorUserId { get; }
    DateTime CreatedDate { get; }
    DateTime? LastModifiedDate { get; }
    
    // Metadata
    string? FreezeNote { get; }
    Dictionary<string, string>? Metadata { get; }
}
