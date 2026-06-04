namespace MyTelegram.Domain.Commands.UserName;

public class CreateUserNameCommand(UserNameId aggregateId, Peer peer, string userName, int date)
    : Command<UserNameAggregate, UserNameId, IExecutionResult>(aggregateId)
{
    public Peer Peer { get; } = peer;
    public string UserName { get; } = userName;
    public int Date { get; } = date;
}