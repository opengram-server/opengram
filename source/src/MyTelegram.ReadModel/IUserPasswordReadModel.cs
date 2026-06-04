namespace MyTelegram.ReadModel;

/// <summary>
/// Interface for user password read model
/// </summary>
public interface IUserPasswordReadModel : IReadModel
{
    long UserId { get; set; }
    bool HasPassword { get; set; }
    bool HasRecovery { get; set; }
    bool HasSecureValues { get; set; }
    string? Hint { get; set; }
    string? RecoveryEmail { get; set; }
    string? EmailUnconfirmedPattern { get; set; }
    string? LoginEmailPattern { get; set; }
    int? PendingResetDate { get; set; }
    byte[]? Salt1 { get; set; }
    byte[]? Salt2 { get; set; }
    int G { get; set; }
    byte[]? P { get; set; }
    byte[]? V { get; set; }
    long? SrpId { get; set; }
    byte[]? SrpB { get; set; }
    byte[]? SrpBPrivate { get; set; }
    string AlgoType { get; set; }
    string SecureAlgoType { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
    DateTime? SrpSessionExpiry { get; set; }
    long? Version { get; set; }
}
