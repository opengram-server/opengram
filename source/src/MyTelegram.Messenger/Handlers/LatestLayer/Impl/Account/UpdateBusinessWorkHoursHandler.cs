using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Shared.Business;
using MyTelegram.Schema;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Updates the <a href="https://corefork.telegram.org/api/business#opening-hours">Telegram Business opening hours</a> of the current user, installable only by users that have Telegram Premium.
/// See <a href="https://corefork.telegram.org/method/account.updateBusinessWorkHours" />
///</summary>
internal sealed class UpdateBusinessWorkHoursHandler(
    ILogger<UpdateBusinessWorkHoursHandler> logger,
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    IUserAppService userAppService) 
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestUpdateBusinessWorkHours, IBool>,
    Account.IUpdateBusinessWorkHoursHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestUpdateBusinessWorkHours obj)
    {
        logger.LogInformation("Updating business work hours for user {UserId}", input.UserId);

        // Check if user has premium/business subscription
        var userReadModel = await userAppService.GetAsync(input.UserId);
        if (userReadModel == null)
        {
            throw new RpcException(RpcErrors.RpcErrors400.UserIdInvalid);
        }

        if (!userReadModel.Premium && !userReadModel.Bot)
        {
            throw new RpcException(RpcErrors.RpcErrors400.PremiumAccountRequired);
        }

        var businessWorkHours = new BusinessWorkHours();

        if (obj.BusinessWorkHours != null)
        {
            businessWorkHours.TimezoneId = obj.BusinessWorkHours.TimezoneId;
            businessWorkHours.OpenNow = obj.BusinessWorkHours.OpenNow;

            if (obj.BusinessWorkHours.WeeklyOpen != null)
            {
                businessWorkHours.WeeklyOpen = obj.BusinessWorkHours.WeeklyOpen
                    .Select(wo => new BusinessWeeklyOpen
                    {
                        StartMinute = wo.StartMinute,
                        EndMinute = wo.EndMinute
                    })
                    .ToList();
            }

            // Validate timezone
            if (!string.IsNullOrEmpty(businessWorkHours.TimezoneId))
            {
                try
                {
                    // Try to find the timezone to validate it exists
                    TimeZoneInfo.FindSystemTimeZoneById(businessWorkHours.TimezoneId);
                }
                catch (TimeZoneNotFoundException)
                {
                    throw new RpcException(RpcErrors.RpcErrors400.TimezoneInvalid);
                }
                catch (InvalidTimeZoneException)
                {
                    throw new RpcException(RpcErrors.RpcErrors400.TimezoneInvalid);
                }
            }

            // Validate weekly open intervals (max 28 intervals)
            if (businessWorkHours.WeeklyOpen.Count > 28)
            {
                throw new RpcException(RpcErrors.RpcErrors400.TimezoneInvalid);
            }

            foreach (var weeklyOpen in businessWorkHours.WeeklyOpen)
            {
                // WeeklyOpen uses minutes from start of week (0-10080 for 7 days)
                const int maxMinutesInWeek = 7 * 24 * 60; // 10080 minutes
                if (weeklyOpen.StartMinute < 0 || weeklyOpen.StartMinute >= maxMinutesInWeek ||
                    weeklyOpen.EndMinute < 0 || weeklyOpen.EndMinute >= maxMinutesInWeek ||
                    weeklyOpen.StartMinute >= weeklyOpen.EndMinute)
                {
                    throw new RpcException(RpcErrors.RpcErrors400.TimezoneInvalid);
                }
            }
        }

        // Create command to update business work hours
        var command = new UpdateBusinessWorkHoursCommand(
            UserId.Create(input.UserId),
            businessWorkHours);

        await commandBus.PublishAsync(command, CancellationToken.None);

        logger.LogInformation("Business work hours updated successfully for user {UserId}", input.UserId);

        return new TBoolTrue();
    }
}
