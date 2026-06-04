namespace MyTelegram.Messenger.Converters.ConverterServices;

public interface IUserConverterService
{
    Task<ILayeredUser> GetUserAsync(IRequestWithAccessHashKeyId request, long userId, bool skipSetContactProperties = true,
        bool skipCheckPrivacy = false, int layer = 0);

    Task<List<ILayeredUser>> GetUserListAsync(IRequestWithAccessHashKeyId request, List<long> userIds, bool skipSetContactProperties = true,
        bool skipCheckPrivacy = false, int layer = 0);

    Task<IUserFull> GetUserFullAsync(IRequestWithAccessHashKeyId request, long userId, int layer = 0);

    IUserFull ToUserFull(IRequestWithAccessHashKeyId request,
        IUserReadModel userReadModel,
        IReadOnlyCollection<IPhotoReadModel>? photoReadModels,
        IReadOnlyCollection<IContactReadModel>? contactReadModels,
        IReadOnlyCollection<IPrivacyReadModel>? privacyReadModels, int layer = 0);
    ILayeredUser ToUser(IRequestWithAccessHashKeyId request, IUserReadModel userReadModel, IReadOnlyCollection<IPhotoReadModel>? photoReadModels = null,
        IContactReadModel? contactReadModel = null, IContactReadModel? targetUserContactReadModel = null, IReadOnlyCollection<IPrivacyReadModel>? privacyReadModels = null, int layer = 0);

    List<ILayeredUser> ToUserList(IRequestWithAccessHashKeyId request, IReadOnlyCollection<IUserReadModel> userReadModels,
        IReadOnlyCollection<IPhotoReadModel> photoReadModels,
        IReadOnlyCollection<IContactReadModel> contactReadModels,
        IReadOnlyCollection<IPrivacyReadModel> privacyReadModels, int layer = 0);
}