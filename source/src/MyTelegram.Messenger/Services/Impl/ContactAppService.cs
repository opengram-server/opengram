using MyTelegram.Schema;
using MyTelegram.Messenger.Services.Interfaces;

namespace MyTelegram.Messenger.Services.Impl;

public interface IContactHelper
{
    MyTelegram.ContactType GetContactType(IContactReadModel? myContactReadModel,
        IContactReadModel? targetUserContactReadModel);

    MyTelegram.ContactType GetContactType(long selfUserId, long targetUserId,
        IReadOnlyCollection<IContactReadModel> contactReadModels);
}

public class ContactAppService(
    IQueryProcessor queryProcessor,
    IPhotoAppService photoAppService,
    IChannelAppService channelAppService,
    IUserAppService userAppService,
    IPeerHelper peerHelper,
    IPrivacyAppService privacyAppService,
    IOptionsMonitor<MyTelegramMessengerServerOptions> options)
    : BaseAppService, IContactAppService, ITransientDependency, IContactHelper
{
    public MyTelegram.ContactType GetContactType(long selfUserId, long targetUserId,
        IReadOnlyCollection<IContactReadModel> contactReadModels)
    {
        var myContactReadModel =
            contactReadModels.FirstOrDefault(p => p.SelfUserId == selfUserId && p.TargetUserId == targetUserId);
        var targetUserContactReadModel =
            contactReadModels.FirstOrDefault(p => p.SelfUserId == targetUserId && p.TargetUserId == selfUserId);

        var contactType = (myContactReadModel, targetUserContactReadModel)
            switch
        {
            { myContactReadModel: not null, targetUserContactReadModel: not null } => MyTelegram.ContactType.Mutual,
            { myContactReadModel: null, targetUserContactReadModel: not null } => MyTelegram.ContactType
                .ContactOfTargetUser,
            { myContactReadModel: not null, targetUserContactReadModel: null } => MyTelegram.ContactType
                .TargetUserIsMyContact,
            _ => MyTelegram.ContactType.None
        };

        return contactType;
    }

    public async Task<MyTelegram.ContactType> GetContactTypeAsync(long selfUserId, long targetUserId)
    {
        var contactReadModels =
            await queryProcessor.ProcessAsync(new GetContactListBySelfIdAndTargetUserIdQuery(selfUserId, targetUserId));

        return GetContactType(selfUserId, targetUserId, contactReadModels);
    }

    public MyTelegram.ContactType GetContactType(IContactReadModel? myContactReadModel, IContactReadModel? targetUserContactReadModel)
    {
        var contactType = (myContactReadModel, targetUserContactReadModel)
            switch
        {
            { myContactReadModel: not null, targetUserContactReadModel: not null } => MyTelegram.ContactType.Mutual,
            { myContactReadModel: null, targetUserContactReadModel: not null } => MyTelegram.ContactType.ContactOfTargetUser,
            { myContactReadModel: not null, targetUserContactReadModel: null } => MyTelegram.ContactType.TargetUserIsMyContact,
            _ => MyTelegram.ContactType.None
        };

        return contactType;
    }

    public async Task<SearchContactOutput> SearchAsync(long selfUserId,
            string keyword, int limit)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            return new SearchContactOutput(selfUserId,
                new List<IUserReadModel>(),
                new List<IPhotoReadModel>(),
                new List<IContactReadModel>(),
                new List<IChannelReadModel>(),
                new List<IChannelReadModel>(),
                new List<IPrivacyReadModel>(),
                new List<IChannelMemberReadModel>());
        }

        var contacts = await queryProcessor.ProcessAsync(new SearchContactQuery(selfUserId, keyword));
        var contactUserIds = contacts.Select(p => p.TargetUserId).ToList();

        var userReadModels = await queryProcessor.ProcessAsync(new GetUsersByUserIdListQuery(contactUserIds));

        var photoIds = userReadModels.Select(p => p.ProfilePhotoId ?? 0).ToList();
        var photoReadModels = await queryProcessor.ProcessAsync(new GetPhotosByPhotoIdLisQuery(photoIds));

        var privacyReadModels = await privacyAppService.GetPrivacyRulesAsync(selfUserId, new TInputPrivacyKeyChatInvite());

        var myTelegramContactList =
            await queryProcessor.ProcessAsync(new GetContactListBySelfUserIdQuery(selfUserId));

        var myContactUserIds = myTelegramContactList.Select(p => p.TargetUserId).ToList();

        var channelIds = contacts.Where(p => p.TargetUserId > 0).Select(p => p.TargetUserId).ToList();

        var channelReadModels = await queryProcessor.ProcessAsync(new GetChannelsByIdsQuery(channelIds));

        var channelMemberReadModels = await queryProcessor.ProcessAsync(new GetChannelMemberByUserIdListQuery(selfUserId, myContactUserIds));

        return new SearchContactOutput(selfUserId,
            userReadModels,
            photoReadModels,
            contacts,
            channelReadModels,
            new List<IChannelReadModel>(),
            new List<IPrivacyReadModel>(),
            new List<IChannelMemberReadModel>());
    }
}
