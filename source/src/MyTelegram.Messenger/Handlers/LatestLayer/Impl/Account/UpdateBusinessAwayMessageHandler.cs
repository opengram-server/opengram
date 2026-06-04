using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// See <a href="https://corefork.telegram.org/method/account.updateBusinessAwayMessage" />
///</summary>
internal sealed class UpdateBusinessAwayMessageHandler(
    ILogger<UpdateBusinessAwayMessageHandler> logger,
    ICommandBus commandBus,
    IUserAppService userAppService) 
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestUpdateBusinessAwayMessage, IBool>,
    Account.IUpdateBusinessAwayMessageHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestUpdateBusinessAwayMessage obj)
    {
        logger.LogInformation("Updating business away message for user {UserId}", input.UserId);

        // Check if user has premium/business subscription
        var userReadModel = await userAppService.GetAsync(input.UserId);
        if (userReadModel == null)
        {
            throw new RpcException(RpcErrors.RpcErrors400.InputUserDeactivated);
        }

        if (!userReadModel.Premium && !userReadModel.Bot)
        {
            throw new RpcException(RpcErrors.RpcErrors400.PremiumAccountRequired);
        }

        BusinessAwayMessage? awayMessage = null;

        if (obj.Message != null)
        {
            awayMessage = new BusinessAwayMessage
            {
                ShortcutId = obj.Message.ShortcutId,
                OfflineOnly = obj.Message.OfflineOnly,
                // TODO: Schedule conversion from Schema to Domain type
                Schedule = new BusinessAwayMessageSchedule
                {
                    Type = obj.Message.Schedule switch
                    {
                        TBusinessAwayMessageScheduleAlways => BusinessAwayMessageScheduleType.Always,
                        TBusinessAwayMessageScheduleOutsideWorkHours => BusinessAwayMessageScheduleType.OutsideWorkHours,
                        TBusinessAwayMessageScheduleCustom => BusinessAwayMessageScheduleType.Custom,
                        _ => BusinessAwayMessageScheduleType.Always
                    },
                    StartMinute = obj.Message.Schedule is TBusinessAwayMessageScheduleCustom customStart ? customStart.StartDate : null,
                    EndMinute = obj.Message.Schedule is TBusinessAwayMessageScheduleCustom customEnd ? customEnd.EndDate : null
                }
            };

            // Convert recipients
            if (obj.Message.Recipients != null)
            {
                awayMessage.Recipients = new BusinessRecipients
                {
                    ExistingChats = obj.Message.Recipients.ExistingChats,
                    NewChats = obj.Message.Recipients.NewChats,
                    Contacts = obj.Message.Recipients.Contacts,
                    NonContacts = obj.Message.Recipients.NonContacts,
                    ExcludeSelected = obj.Message.Recipients.ExcludeSelected,
                    Users = obj.Message.Recipients.Users?.Select(u => u switch
                    {
                        TInputUserSelf => input.UserId,
                        TInputUser inputUser => inputUser.UserId,
                        _ => 0L
                    }).Where(id => id != 0).ToList() ?? new List<long>()
                };
            }
        }

        // Create command to update business away message
        var command = new UpdateBusinessAwayMessageCommand(
            UserId.Create(input.UserId),
            awayMessage);

        await commandBus.PublishAsync(command, CancellationToken.None);

        logger.LogInformation("Business away message updated successfully for user {UserId}", input.UserId);

        return new TBoolTrue();
    }
}
