namespace MyTelegram.QueryHandlers.MongoDB.User;

public class GetUserByPhoneNumberQueryHandler(IQueryOnlyReadModelStore<UserReadModel> store) : IQueryHandler<GetUserByPhoneNumberQuery, IUserReadModel?>
{
    public async Task<IUserReadModel?> ExecuteQueryAsync(GetUserByPhoneNumberQuery query,
        CancellationToken cancellationToken)
    {
        var phoneNumber = query.PhoneNumber.Replace("+", string.Empty).Replace(" ", string.Empty);
        return await store
            .FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber, cancellationToken: cancellationToken);
    }
}
