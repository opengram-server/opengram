using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Shared.Business;
using MyTelegram.Schema;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Changes the <a href="https://corefork.telegram.org/api/business#greeting-messages">Telegram Business greeting message</a> of the current user, installable only by users that have Telegram Premium.
/// See <a href="https://corefork.telegram.org/method/account.updateBusinessGreetingMessage" />
///</summary>
internal sealed class UpdateBusinessGreetingMessageHandler(
    ILogger<UpdateBusinessGreetingMessageHandler> logger,
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    IUserAppService userAppService) 
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestUpdateBusinessGreetingMessage, IBool>,
    Account.IUpdateBusinessGreetingMessageHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestUpdateBusinessGreetingMessage obj)
    {
        logger.LogInformation("Updating business greeting message for user {UserId}", input.UserId);

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

        BusinessGreetingMessage greetingMessage = null;

        if (obj.Message != null)
        {
            // Note: Quick reply validation can be added later if needed

            greetingMessage = new BusinessGreetingMessage
            {
                ShortcutId = obj.Message.ShortcutId,
                NoActivityDays = obj.Message.NoActivityDays
            };

            // Validate no activity days (must be 7, 14, 21, or 28)
            var validDays = new[] { 7, 14, 21, 28 };
            if (!validDays.Contains(greetingMessage.NoActivityDays))
            {
                throw new RpcException(RpcErrors.RpcErrors400.DataInvalid);
            }

            // Convert recipients
            if (obj.Message.Recipients != null)
            {
                greetingMessage.Recipients = new BusinessRecipients
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

        // Create command to update business greeting message
        var command = new UpdateBusinessGreetingMessageCommand(
            UserId.Create(input.UserId),
            greetingMessage);

        await commandBus.PublishAsync(command, CancellationToken.None);

        logger.LogInformation("Business greeting message updated successfully for user {UserId}", input.UserId);

        return new TBoolTrue();
    }
}
