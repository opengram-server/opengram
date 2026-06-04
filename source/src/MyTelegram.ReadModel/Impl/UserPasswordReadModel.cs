using MyTelegram.Domain.Aggregates.UserPassword;
using MyTelegram.Domain.Events.UserPassword;

namespace MyTelegram.ReadModel.Impl;

/// <summary>
/// ReadModel for storing user's 2FA password information
/// </summary>
public class UserPasswordReadModel : IUserPasswordReadModel,
    IAmReadModelFor<UserPasswordAggregate, UserPasswordId, PasswordSetEvent>,
    IAmReadModelFor<UserPasswordAggregate, UserPasswordId, PasswordRemovedEvent>,
    IAmReadModelFor<UserPasswordAggregate, UserPasswordId, SrpSessionCreatedEvent>,
    IAmReadModelFor<UserPasswordAggregate, UserPasswordId, PasswordVerifiedEvent>
{
    /// <summary>
    /// Unique identifier (UserId as string)
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// User ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Whether user has password enabled
    /// </summary>
    public bool HasPassword { get; set; }

    /// <summary>
    /// Whether user has recovery email set
    /// </summary>
    public bool HasRecovery { get; set; }

    /// <summary>
    /// Whether user has secure values (Telegram Passport)
    /// </summary>
    public bool HasSecureValues { get; set; }

    /// <summary>
    /// Password hint (visible to user)
    /// </summary>
    public string? Hint { get; set; }

    /// <summary>
    /// Recovery email address
    /// </summary>
    public string? RecoveryEmail { get; set; }

    /// <summary>
    /// Pattern for unconfirmed email (e.g., "t***@example.com")
    /// </summary>
    public string? EmailUnconfirmedPattern { get; set; }

    /// <summary>
    /// Pattern for login email (e.g., "u***@gmail.com")
    /// </summary>
    public string? LoginEmailPattern { get; set; }

    /// <summary>
    /// Pending reset date (Unix timestamp) - when password will be auto-removed
    /// </summary>
    public int? PendingResetDate { get; set; }

    /// <summary>
    /// SRP salt 1 for current password
    /// </summary>
    public byte[]? Salt1 { get; set; }

    /// <summary>
    /// SRP salt 2 for current password
    /// </summary>
    public byte[]? Salt2 { get; set; }

    /// <summary>
    /// SRP parameter g (generator)
    /// </summary>
    public int G { get; set; }

    /// <summary>
    /// SRP parameter p (large prime)
    /// </summary>
    public byte[]? P { get; set; }

    /// <summary>
    /// SRP parameter v (password verifier)
    /// </summary>
    public byte[]? V { get; set; }

    /// <summary>
    /// Current SRP session ID (changes on each getPassword call)
    /// </summary>
    public long? SrpId { get; set; }

    /// <summary>
    /// Current SRP B parameter (server public value)
    /// </summary>
    public byte[]? SrpB { get; set; }

    /// <summary>
    /// SRP b parameter (server private value) - stored temporarily for session
    /// </summary>
    public byte[]? SrpBPrivate { get; set; }

    /// <summary>
    /// Algorithm type identifier
    /// </summary>
    public string AlgoType { get; set; } = "passwordKdfAlgoSHA256SHA256PBKDF2HMACSHA512iter100000SHA256ModPow";

    /// <summary>
    /// Secure password KDF algorithm for Telegram Passport
    /// </summary>
    public string SecureAlgoType { get; set; } = "securePasswordKdfAlgoUnknown";

    /// <summary>
    /// When password was created/last updated
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When password was last modified
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// SRP session expiration timestamp
    /// </summary>
    public DateTime? SrpSessionExpiry { get; set; }

    /// <summary>
    /// Version for optimistic locking
    /// </summary>
    public long? Version { get; set; }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserPasswordAggregate, UserPasswordId, PasswordSetEvent> domainEvent, CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        Id = domainEvent.AggregateIdentity.Value;
        UserId = evt.UserId;
        HasPassword = true;
        Salt1 = evt.Salt1;
        Salt2 = evt.Salt2;
        V = evt.V;
        Hint = evt.Hint;
        RecoveryEmail = evt.Email;
        HasRecovery = !string.IsNullOrEmpty(evt.Email);
        G = evt.G;
        P = evt.P;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserPasswordAggregate, UserPasswordId, PasswordRemovedEvent> domainEvent, CancellationToken cancellationToken)
    {
        HasPassword = false;
        Salt1 = null;
        Salt2 = null;
        V = null;
        Hint = null;
        SrpId = null;
        SrpB = null;
        SrpBPrivate = null;
        UpdatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserPasswordAggregate, UserPasswordId, SrpSessionCreatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        SrpId = evt.SrpId;
        SrpB = evt.SrpB;
        SrpBPrivate = evt.SrpBPrivate;
        SrpSessionExpiry = DateTime.UtcNow.AddMinutes(5); // SRP session expires in 5 minutes
        UpdatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserPasswordAggregate, UserPasswordId, PasswordVerifiedEvent> domainEvent, CancellationToken cancellationToken)
    {
        // Clear SRP session after successful verification
        SrpId = null;
        SrpB = null;
        SrpBPrivate = null;
        SrpSessionExpiry = null;
        UpdatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }
}
