using System.Text;

namespace MyTelegram.Domain.Commands.User;

public class CreateUserCommand(
    UserId aggregateId,
    RequestInfo requestInfo,
    long userId,
    long accessHash,
    string phoneNumber,
    string firstName,
    string? lastName,
    string? userName = null,
    bool bot = false)
    : RequestCommand2<UserAggregate, UserId, IExecutionResult>(aggregateId, requestInfo)
{
    public long AccessHash { get; } = accessHash;
    public bool Bot { get; } = bot;
    public string FirstName { get; } = firstName;
    public string? LastName { get; } = lastName;
    public string? UserName { get; } = userName;
    public string PhoneNumber { get; } = phoneNumber;
    public long UserId { get; } = userId;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        if (Bot)
        {
            yield return BitConverter.GetBytes(UserId);
            yield return Encoding.UTF8.GetBytes(UserName!);
        }
        else
        {
            yield return BitConverter.GetBytes(RequestInfo.ReqMsgId);
            yield return Encoding.UTF8.GetBytes(PhoneNumber);
        }
    }
}
