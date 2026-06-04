using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Messenger.Services.Interfaces;

public interface IContactAppService
{
    MyTelegram.ContactType GetContactType(long selfUserId, long targetUserId,
        System.Collections.Generic.IReadOnlyCollection<IContactReadModel> contactReadModels);

    System.Threading.Tasks.Task<MyTelegram.ContactType> GetContactTypeAsync(long selfUserId, long targetUserId);

    System.Threading.Tasks.Task<SearchContactOutput> SearchAsync(long selfUserId,
            string keyword, int limit);
}