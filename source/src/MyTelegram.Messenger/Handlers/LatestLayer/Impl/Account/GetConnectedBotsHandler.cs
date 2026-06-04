using Microsoft.Extensions.Logging;
using MyTelegram.Schema;
using MyTelegram.Queries.Business;
using MyTelegram.Domain.Shared.Business;
using MyTelegram.Messenger.Services.Impl;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// List all currently connected <a href="https://corefork.telegram.org/api/business#connected-bots">business bots »</a>
/// See <a href="https://corefork.telegram.org/method/account.getConnectedBots" />
///</summary>
internal sealed class GetConnectedBotsHandler(
    ILogger<GetConnectedBotsHandler> logger,
    IQueryProcessor queryProcessor,
    IUserAppService userAppService) 
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestGetConnectedBots, MyTelegram.Schema.Account.IConnectedBots>,
    Account.IGetConnectedBotsHandler
{
    protected override async Task<MyTelegram.Schema.Account.IConnectedBots> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestGetConnectedBots obj)
    {
        logger.LogInformation("Getting connected bots for user {UserId}", input.UserId);

        // Check if user has premium/business subscription
        var userReadModel = await userAppService.GetAsync(input.UserId);
        if (userReadModel == null)
            {
                throw new RpcException(RpcErrors.RpcErrors400.InputUserDeactivated);
            }

        if (!userReadModel.Premium && !userReadModel.Bot)
        {
            throw new RpcException(RpcErrors.RpcErrors403.PremiumAccountRequired);
        }

        // Get connected bots
        var connectedBotsList = await queryProcessor.ProcessAsync(new MyTelegram.Queries.Business.GetConnectedBotsQuery(input.UserId));

        // Load user information for the bots  
        var botUserIds = connectedBotsList.Select(cb => cb.BotId).ToList();
        var botUsers = new List<IUser>();

        if (botUserIds.Any())
        {
            var usersQuery = await queryProcessor.ProcessAsync(new GetUserListByIdQuery(botUserIds));
            if (usersQuery != null)
            {
                botUsers = usersQuery.Cast<IUser>().ToList();
            }
        }

        var connectedBotsVector = new TVector<MyTelegram.Schema.IConnectedBot>(
            connectedBotsList.Select(cb => (MyTelegram.Schema.IConnectedBot)new MyTelegram.Schema.TConnectedBot
            {
                BotId = cb.BotId,
                Recipients = new MyTelegram.Schema.TBusinessBotRecipients
                {
                    ExistingChats = false,
                    NewChats = false,
                    Contacts = false,
                    NonContacts = false,
                    ExcludeSelected = false,
                    Users = cb.Recipients != null ? new TVector<long>(cb.Recipients) : new TVector<long>()
                },
                Rights = new MyTelegram.Schema.TBusinessBotRights
                {
                    Reply = cb.CanReply
                }
            }));

        var result = new TConnectedBots
        {
            ConnectedBots = connectedBotsVector,
            Users = new TVector<IUser>(botUsers)
        };

        logger.LogInformation("Retrieved {Count} connected bots for user {UserId}", connectedBotsList.Count, input.UserId);

        return result;
    }
}
