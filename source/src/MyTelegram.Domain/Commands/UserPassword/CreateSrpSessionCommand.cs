using MyTelegram.Domain.Aggregates.UserPassword;

namespace MyTelegram.Domain.Commands.UserPassword;

/// <summary>
/// Command to create SRP session for password verification
/// </summary>
public class CreateSrpSessionCommand : RequestCommand2<UserPasswordAggregate, UserPasswordId, IExecutionResult>
{
    public CreateSrpSessionCommand(
        UserPasswordId aggregateId,
        RequestInfo requestInfo,
        long userId,
        long srpId,
        byte[] srpB,
        byte[] srpBPrivate) : base(aggregateId, requestInfo)
    {
        UserId = userId;
        SrpId = srpId;
        SrpB = srpB;
        SrpBPrivate = srpBPrivate;
    }

    public long UserId { get; }
    
    /// <summary>
    /// SRP session ID
    /// </summary>
    public long SrpId { get; }

    /// <summary>
    /// SRP server public value (B)
    /// </summary>
    public byte[] SrpB { get; }

    /// <summary>
    /// SRP server private value (b) - for verification
    /// </summary>
    public byte[] SrpBPrivate { get; }
}
