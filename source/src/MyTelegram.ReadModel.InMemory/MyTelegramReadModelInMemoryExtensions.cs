using EventFlow.Queries;
using EventFlow.ReadStores.InMemory.Queries;
using MyTelegram.Domain.Aggregates.AppCode;
using MyTelegram.Domain.Aggregates.Channel;
using MyTelegram.Domain.Aggregates.ChatInvite;
using MyTelegram.Domain.Aggregates.Contact;
using MyTelegram.Domain.Aggregates.Device;
using MyTelegram.Domain.Aggregates.Dialog;
using MyTelegram.Domain.Aggregates.Document;
using MyTelegram.Domain.Aggregates.Language;
using MyTelegram.Domain.Aggregates.Messaging;
using MyTelegram.Domain.Aggregates.PeerNotifySetting;
using MyTelegram.Domain.Aggregates.PeerSetting;
using MyTelegram.Domain.Aggregates.Photo;
using MyTelegram.Domain.Aggregates.Poll;
using MyTelegram.Domain.Aggregates.Pts;
using MyTelegram.Domain.Aggregates.PushDevice;
using MyTelegram.Domain.Aggregates.RpcResult;
using MyTelegram.Domain.Aggregates.Updates;
using MyTelegram.Domain.Aggregates.UserName;
using MyTelegram.EventFlow.ReadStores;
using MyTelegram.ReadModel.Extensions;

namespace MyTelegram.ReadModel.InMemory;

public static class MyTelegramReadModelInMemoryExtensions
{
    public static IEventFlowOptions
        UseMyInMemoryReadStoreFor<TAggregate, TIdentity, TReadModel>(this IEventFlowOptions options)
        where TReadModel : class, IReadModel
    {
        options.UseInMemoryReadStoreFor<TReadModel>();

        options.ServiceCollection.AddSingleton<IMyInMemoryReadStore<TReadModel>, MyInMemoryReadStore<TReadModel>>();

        options.ServiceCollection.AddSingleton<IInMemoryReadStore<TReadModel>>(r =>
            r.GetRequiredService<IMyInMemoryReadStore<TReadModel>>());
        options.ServiceCollection.AddSingleton<IQueryOnlyReadModelStore<TReadModel>>(r =>
            r.GetRequiredService<IMyInMemoryReadStore<TReadModel>>());

        options.ServiceCollection.AddSingleton<IInMemoryReadStore<TReadModel>>(r =>
            r.GetRequiredService<IMyInMemoryReadStore<TReadModel>>());
        options.ServiceCollection.AddSingleton<IReadModelStore<TReadModel>>(r =>
            r.GetRequiredService<IMyInMemoryReadStore<TReadModel>>());

        options.ServiceCollection
            .AddTransient<IQueryHandler<InMemoryQuery<TReadModel>, IReadOnlyCollection<TReadModel>>,
                InMemoryQueryHandler<TReadModel>>();

        return options;
    }

    public static IEventFlowOptions
        UseMyInMemoryReadStoreFor<TReadModel, TReadModelLocator>(this IEventFlowOptions options)
        where TReadModel : class, IReadModel where TReadModelLocator : IReadModelLocator
    {
        options.UseInMemoryReadStoreFor<TReadModel, TReadModelLocator>();

        options.ServiceCollection.AddSingleton<IMyInMemoryReadStore<TReadModel>, MyInMemoryReadStore<TReadModel>>();

        options.ServiceCollection.AddSingleton<IInMemoryReadStore<TReadModel>>(r =>
            r.GetRequiredService<IMyInMemoryReadStore<TReadModel>>());
        options.ServiceCollection.AddSingleton<IQueryOnlyReadModelStore<TReadModel>>(r =>
            r.GetRequiredService<IMyInMemoryReadStore<TReadModel>>());

        options.ServiceCollection.AddSingleton<IInMemoryReadStore<TReadModel>>(r =>
            r.GetRequiredService<IMyInMemoryReadStore<TReadModel>>());
        options.ServiceCollection.AddSingleton<IReadModelStore<TReadModel>>(r =>
            r.GetRequiredService<IMyInMemoryReadStore<TReadModel>>());
        options.ServiceCollection.AddSingleton<IQueryOnlyReadModelStore<TReadModel>>(r => r.GetRequiredService<IMyInMemoryReadStore<TReadModel>>());

        options.ServiceCollection
            .AddTransient<IQueryHandler<InMemoryQuery<TReadModel>, IReadOnlyCollection<TReadModel>>,
                InMemoryQueryHandler<TReadModel>>();

        return options;
    }

    public static IEventFlowOptions AddMessengerInMemoryReadModel(this IEventFlowOptions options)
    {
        options.ServiceCollection.AddMyTelegramReadModel();
        options.UseMyInMemoryReadStoreFor<UpdatesAggregate, UpdatesId, UpdatesReadModel>();
        options.UseMyInMemoryReadStoreFor<RpcResultAggregate, RpcResultId, RpcResultReadModel>();
        options.UseMyInMemoryReadStoreFor<PtsAggregate, PtsId, PtsReadModel>();
        options.UseMyInMemoryReadStoreFor<PtsAggregate, PtsId, PtsForAuthKeyIdReadModel>();
        //options.UseMyInMemoryReadStoreFor<GroupCallParticipantAggregate, GroupCallParticipantId, GroupCallParticipantReadModel>();

        options.AddDefaults(typeof(MyTelegramReadModelInMemoryExtensions).Assembly)
            .UseMyInMemoryReadStoreFor<AppCodeAggregate, AppCodeId, AppCodeReadModel>()
            .UseMyInMemoryReadStoreFor<DialogReadModel, IDialogReadModelLocator>()
            .UseMyInMemoryReadStoreFor<MessageReadModel, IMessageIdLocator>()
            .UseMyInMemoryReadStoreFor<PeerNotifySettingsAggregate, PeerNotifySettingsId, PeerNotifySettingsReadModel>()
            .UseMyInMemoryReadStoreFor<UserReadModel, IUserReadModelLocator>()
            .UseMyInMemoryReadStoreFor<ChannelReadModel, IChannelReadModelLocator>()
            .UseMyInMemoryReadStoreFor<ChannelFullReadModel, IChannelFullReadModelLocator>()
            .UseMyInMemoryReadStoreFor<ChannelMemberAggregate, ChannelMemberId, ChannelMemberReadModel>()
            .UseMyInMemoryReadStoreFor<UserNameAggregate, UserNameId, UserNameReadModel>()
            .UseMyInMemoryReadStoreFor<DeviceAggregate, DeviceId, DeviceReadModel>()
            .UseMyInMemoryReadStoreFor<PushDeviceAggregate, PushDeviceId, PushDeviceReadModel>()
            .UseMyInMemoryReadStoreFor<DraftReadModel, DraftReadModelLocator>()
            .UseMyInMemoryReadStoreFor<ContactAggregate, ContactId, ContactReadModel>()
            .UseMyInMemoryReadStoreFor<ImportedContactAggregate, ImportedContactId, ImportedContactReadModel>()
            .UseMyInMemoryReadStoreFor<ReadingHistoryAggregate, ReadingHistoryId, ReadingHistoryReadModel>()
            .UseMyInMemoryReadStoreFor<ReplyReadModel, IReplyReadModelLocator>()
            .UseMyInMemoryReadStoreFor<DialogFilterAggregate, DialogFilterId, DialogFilterReadModel>()
            .UseMyInMemoryReadStoreFor<PollAggregate, PollId, PollReadModel>()
            .UseMyInMemoryReadStoreFor<PollAnswerVoterReadModel, IPollAnswerVoterReadModelLocator>()
            .UseMyInMemoryReadStoreFor<AccessHashReadModel, IAccessHashReadModelLocator>()
            .UseMyInMemoryReadStoreFor<PeerSettingsAggregate, PeerSettingsId, PeerSettingsReadModel>()
            .UseMyInMemoryReadStoreFor<ChatAdminReadModel, IChatAdminReadModelLocator>()
            .UseMyInMemoryReadStoreFor<ChatInviteImporterReadModel, IChatInviteImporterReadModelLocator>()
            .UseMyInMemoryReadStoreFor<ChatInviteAggregate, ChatInviteId, ChatInviteReadModel>()
            .UseMyInMemoryReadStoreFor<PhotoAggregate, PhotoId, PhotoReadModel>()
            .UseMyInMemoryReadStoreFor<DocumentAggregate, DocumentId, DocumentReadModel>()
            .UseMyInMemoryReadStoreFor<PtsAggregate, PtsId, PtsReadModel>()
            .UseMyInMemoryReadStoreFor<PtsAggregate, PtsId, PtsForAuthKeyIdReadModel>()
            .UseMyInMemoryReadStoreFor<LanguageAggregate, LanguageId, LanguageReadModel>()
            .UseMyInMemoryReadStoreFor<LanguageTextAggregate, LanguageTextId, LanguageTextReadModel>()
            .UseMyInMemoryReadStoreFor<JoinChannelAggregate, JoinChannelId, JoinChannelRequestReadModel>()
            ;

        return options;
    }
}