using MyTelegram.Domain;

namespace MyTelegram.ReadModel.Interfaces;

public interface IUserFrozenAppealHistoryReadModel : IReadModel
{
    string Id { get; }
    long UserId { get; }
    string AppealId { get; }
    
    DateTime SubmittedDate { get; }
    string AppealText { get; }
    string? CaptchaToken { get; }
    
    AppealStatus Status { get; }
    DateTime? ReviewedDate { get; }
    long? ReviewerModeratorId { get; }
    string? ReviewNote { get; }
    
    string? UserName { get; }
    Dictionary<string, string>? Answers { get; }
}
