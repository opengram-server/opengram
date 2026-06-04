// ReSharper disable once CheckNamespace
namespace MyTelegram.Domain;

public enum FreezeReason
{
    Unknown = 0,
    Spam = 1,
    MaliciousLinks = 2,
    MassReports = 3,
    TosViolation = 4,
    Other = 99
}
