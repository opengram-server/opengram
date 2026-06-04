namespace MyTelegram.Queries;

public class GetBotVerificationByTargetQuery : IQuery<IBotVerificationReadModel?>
{
    public GetBotVerificationByTargetQuery(VerificationTargetType targetType, long targetId)
    {
        TargetType = targetType;
        TargetId = targetId;
    }

    public VerificationTargetType TargetType { get; }
    public long TargetId { get; }
}
