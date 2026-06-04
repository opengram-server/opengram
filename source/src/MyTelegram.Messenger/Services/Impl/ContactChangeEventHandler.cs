using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Messenger.Services.Impl;

/// <summary>
/// Interface for handling integration events
/// </summary>
public interface IIntegrationEventHandler<T>
{
    Task HandleAsync(T eventData);
}

/// <summary>
/// Handles contact changes to update Action Bar settings
/// </summary>
public class ContactChangeEventHandler(
    ILogger<ContactChangeEventHandler> logger,
    IActionBarAppService actionBarAppService,
    ICommandBus commandBus,
    IQueryProcessor queryProcessor) : IIntegrationEventHandler<ContactAddedEvent>, IIntegrationEventHandler<ContactDeletedEvent>
{
    public async Task HandleAsync(ContactAddedEvent integrationEvent)
    {
        logger.LogDebug("Contact added: {SelfUserId} -> {TargetUserId}", integrationEvent.SelfUserId, integrationEvent.TargetUserId);

        // Get current peer settings
        var peerSettingsReadModel = await queryProcessor.ProcessAsync(
            new GetPeerSettingsQuery(integrationEvent.SelfUserId, integrationEvent.TargetUserId));

        if (peerSettingsReadModel?.PeerSettings != null)
        {
            var oldContactType = DetermineOldContactType(integrationEvent.SelfUserId, integrationEvent.TargetUserId);
            var newContactType = MyTelegram.ContactType.TargetUserIsMyContact;

            var updatedSettings = actionBarAppService.UpdatePeerSettingsOnContactChange(
                peerSettingsReadModel.PeerSettings, oldContactType, newContactType);

            // Create command to update peer settings if needed
            if (SettingsChanged(peerSettingsReadModel.PeerSettings, updatedSettings))
            {
                var localSettings = new MyTelegram.Messenger.Services.Impl.PeerSettings();
                var updateCommand = new UpdatePeerSettingsCommand(
                    MyTelegram.Domain.Aggregates.PeerSetting.PeerSettingsId.Create(integrationEvent.SelfUserId, integrationEvent.TargetUserId),
                    new RequestInfo(integrationEvent.SelfUserId, Guid.NewGuid().ToString()),
                    integrationEvent.TargetUserId,
                    localSettings);

                await commandBus.PublishAsync(updateCommand);
                logger.LogDebug("Updated peer settings for contact addition: {SelfUserId} -> {TargetUserId}", 
                    integrationEvent.SelfUserId, integrationEvent.TargetUserId);
            }
        }
    }

    public async Task HandleAsync(ContactDeletedEvent integrationEvent)
    {
        // For ContactDeletedEvent, we need to determine selfUserId from the aggregate context
        // This is a limitation of the current event structure
        logger.LogDebug("Contact deleted for target: {TargetUid}", integrationEvent.TargetUid);

        // Note: SelfUserId is not available in ContactDeletedEvent, need to get from context
        // For now, we'll use a placeholder approach
        var selfUserId = integrationEvent.RequestInfo.UserId;

        // Get current peer settings
        var peerSettingsReadModel = await queryProcessor.ProcessAsync(
            new GetPeerSettingsQuery(selfUserId, integrationEvent.TargetUid));

        if (peerSettingsReadModel?.PeerSettings != null)
        {
            var oldContactType = MyTelegram.ContactType.TargetUserIsMyContact;
            var newContactType = MyTelegram.ContactType.None;

            var updatedSettings = actionBarAppService.UpdatePeerSettingsOnContactChange(
                peerSettingsReadModel.PeerSettings, oldContactType, newContactType);

            // Create command to update peer settings if needed
            if (SettingsChanged(peerSettingsReadModel.PeerSettings, updatedSettings))
            {
                var localSettings = new MyTelegram.Messenger.Services.Impl.PeerSettings();
                var updateCommand = new UpdatePeerSettingsCommand(
                    MyTelegram.Domain.Aggregates.PeerSetting.PeerSettingsId.Create(selfUserId, integrationEvent.TargetUid),
                    new RequestInfo(selfUserId, Guid.NewGuid().ToString()),
                    integrationEvent.TargetUid,
                    localSettings);

                await commandBus.PublishAsync(updateCommand);
                logger.LogDebug("Updated peer settings for contact deletion: {SelfUserId} -> {TargetUserId}", 
                    selfUserId, integrationEvent.TargetUid);
            }
        }
    }

    private MyTelegram.ContactType DetermineOldContactType(long selfUserId, long targetUserId)
    {
        // Before adding contact, check if target user has us in their contacts
        // This would typically involve a database query, but for simplicity we assume None
        return MyTelegram.ContactType.None;
    }

    private bool SettingsChanged(MyTelegram.PeerSettings oldSettings, MyTelegram.PeerSettings newSettings)
    {
        return oldSettings.ReportSpam != newSettings.ReportSpam ||
               oldSettings.AddContact != newSettings.AddContact ||
               oldSettings.BlockContact != newSettings.BlockContact ||
               oldSettings.ShareContact != newSettings.ShareContact;
    }
}
