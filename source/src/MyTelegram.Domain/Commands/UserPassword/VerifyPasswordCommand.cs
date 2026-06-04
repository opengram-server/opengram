using MyTelegram.Domain.Aggregates.UserPassword;

namespace MyTelegram.Domain.Commands.UserPassword;

/// <summary>
/// Command to verify password using SRP
/// </summary>
public class VerifyPasswordCommand : RequestCommand2<UserPasswordAggregate, UserPasswordId, IExecutionResult>
{
    public VerifyPasswordCommand(
        UserPasswordId aggregateId,
        RequestInfo requestInfo,
        long userId,
        long srpId,
        byte[] clientA,
        byte[] clientM1) : base(aggregateId, requestInfo)
    {
        UserId = userId;
        SrpId = srpId;
        ClientA = clientA;
        ClientM1 = clientM1;
    }

    public long UserId { get; }
    
    /// <summary>
    /// SRP session ID from client
    /// </summary>
    public long SrpId { get; }

    /// <summary>
    /// Client's public value (A)
    /// </summary>
    public byte[] ClientA { get; }

    /// <summary>
    /// Client's proof (M1)
    /// </summary>
    public byte[] ClientM1 { get; }
}
