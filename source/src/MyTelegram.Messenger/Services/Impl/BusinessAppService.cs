using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Shared.Business;
using MyTelegram.Schema;
using MyTelegram.Domain.Aggregates.Channel;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Queries;
using EventFlow;
using MyTelegram.Domain.Aggregates.User;
using MyTelegram.Domain.Commands.User;
using MyTelegram.Queries.Business;

namespace MyTelegram.Messenger.Services.Impl;

/// <summary>
/// Service for managing Telegram Business features
/// </summary>
public interface IBusinessAppService
{
    /// <summary>
    /// Gets user's business configuration
    /// </summary>
    Task<UserBusinessConfig> GetBusinessConfigAsync(long userId);

    /// <summary>
    /// Updates user's business work hours
    /// </summary>
    Task UpdateBusinessWorkHoursAsync(long userId, BusinessWorkHours workHours);

    /// <summary>
    /// Updates user's business location
    /// </summary>
    Task UpdateBusinessLocationAsync(long userId, BusinessLocation location);

    /// <summary>
    /// Updates user's business greeting message
    /// </summary>
    Task UpdateBusinessGreetingMessageAsync(long userId, BusinessGreetingMessage greetingMessage);

    /// <summary>
    /// Updates user's business intro
    /// </summary>
    Task UpdateBusinessIntroAsync(long userId, BusinessIntro businessIntro);

    /// <summary>
    /// Creates a business chat link
    /// </summary>
    Task<BusinessChatLink> CreateBusinessChatLinkAsync(long userId, string title, string message, List<string> entities);

    /// <summary>
    /// Gets all business chat links for user
    /// </summary>
    Task<List<BusinessChatLink>> GetBusinessChatLinksAsync(long userId);

    /// <summary>
    /// Deletes a business chat link
    /// </summary>
    Task DeleteBusinessChatLinkAsync(long userId, string linkId);

    /// <summary>
    /// Determines if greeting message should be sent based on chat history
    /// </summary>
    Task<bool> ShouldSendGreetingMessageAsync(long userId, long peerId, DateTime lastActivity);

    /// <summary>
    /// Determines if away message should be sent based on schedule and online status
    /// </summary>
    Task<bool> ShouldSendAwayMessageAsync(long userId, DateTime currentTime);

    /// <summary>
    /// Gets business opening hours with current status
    /// </summary>
    Task<BusinessOpeningHours> GetCurrentBusinessHoursAsync(long userId, DateTime currentTime);
}

public class UserBusinessConfig
{
    public BusinessWorkHours? WorkHours { get; set; }
    public BusinessLocation? Location { get; set; }
    public BusinessGreetingMessage? GreetingMessage { get; set; }
    public BusinessIntro? Intro { get; set; }
    public BusinessAwayMessage? AwayMessage { get; set; }
    public List<BusinessChatLink> ChatLinks { get; set; } = new();
    public bool IsBusinessAccount { get; set; }
}

public class BusinessOpeningHours
{
    public bool IsOpen { get; set; }
    public string TimezoneName { get; set; } = string.Empty;
    public List<BusinessWeeklyOpen> TodayHours { get; set; } = new();
    public DateTime? NextOpeningTime { get; set; }
    public DateTime? NextClosingTime { get; set; }
}

public class BusinessAppService(
    ILogger<BusinessAppService> logger,
    IQueryProcessor queryProcessor,
    ICommandBus commandBus,
    IContactAppService contactAppService) : IBusinessAppService, ITransientDependency
{
    private readonly IContactAppService _contactAppService = contactAppService;
    public async Task<UserBusinessConfig> GetBusinessConfigAsync(long userId)
    {
        logger.LogDebug("Getting business config for user {UserId}", userId);

        var userFullReadModel = await queryProcessor.ProcessAsync<IUserFullReadModel>(new GetUserFullQuery(userId), CancellationToken.None);
        
        return new UserBusinessConfig
        {
            WorkHours = userFullReadModel?.BusinessWorkHours != null ? MapToDomain(userFullReadModel.BusinessWorkHours) : null,
            Location = userFullReadModel?.BusinessLocation != null ? MapToDomain(userFullReadModel.BusinessLocation) : null,
            GreetingMessage = userFullReadModel?.BusinessGreetingMessage != null ? MapToDomain(userFullReadModel.BusinessGreetingMessage) : null,
            Intro = userFullReadModel?.BusinessIntro != null ? MapToDomain(userFullReadModel.BusinessIntro) : null,
            AwayMessage = userFullReadModel?.BusinessAwayMessage != null ? MapToDomain(userFullReadModel.BusinessAwayMessage) : null,
            IsBusinessAccount = userFullReadModel?.IsBusinessAccount ?? false
        };
    }

    public async Task UpdateBusinessWorkHoursAsync(long userId, BusinessWorkHours workHours)
    {
        logger.LogInformation("Updating business work hours for user {UserId}", userId);

        var command = new UpdateBusinessWorkHoursCommand(
            UserId.Create(userId),
            workHours);

        await commandBus.PublishAsync(command, CancellationToken.None);
    }

    public async Task UpdateBusinessLocationAsync(long userId, BusinessLocation location)
    {
        logger.LogInformation("Updating business location for user {UserId}", userId);

        var command = new UpdateBusinessLocationCommand(
            UserId.Create(userId),
            location);

        await commandBus.PublishAsync(command, CancellationToken.None);
    }

    public async Task UpdateBusinessGreetingMessageAsync(long userId, BusinessGreetingMessage greetingMessage)
    {
        logger.LogInformation("Updating business greeting message for user {UserId}", userId);

        var command = new UpdateBusinessGreetingMessageCommand(
            UserId.Create(userId),
            greetingMessage);

        await commandBus.PublishAsync(command, CancellationToken.None);
    }

    public async Task UpdateBusinessIntroAsync(long userId, BusinessIntro businessIntro)
    {
        logger.LogInformation("Updating business intro for user {UserId}", userId);

        var command = new UpdateBusinessIntroCommand(
            UserId.Create(userId),
            businessIntro);

        await commandBus.PublishAsync(command, CancellationToken.None);
    }

    public async Task<BusinessChatLink> CreateBusinessChatLinkAsync(long userId, string title, string message, List<string> entities)
    {
        logger.LogInformation("Creating business chat link for user {UserId}", userId);

        var linkId = GenerateLinkId();
        var link = $"https://t.me/{userId}?business={linkId}";

        var businessChatLink = new BusinessChatLink
        {
            Id = linkId,
            Link = link,
            Title = title,
            Message = message,
            Entities = entities?.Select(e => new MyTelegram.Domain.Shared.Business.MessageEntity { Type = "text", Offset = 0, Length = e.Length }).ToList() ?? new List<MyTelegram.Domain.Shared.Business.MessageEntity>(),
            Views = 0
        };

        var command = new CreateBusinessChatLinkCommand(
            UserId.Create(userId),
            businessChatLink);

        await commandBus.PublishAsync(command, CancellationToken.None);

        return businessChatLink;
    }

    public async Task<List<BusinessChatLink>> GetBusinessChatLinksAsync(long userId)
    {
        logger.LogDebug("Getting business chat links for user {UserId}", userId);

        var linksResult = await queryProcessor.ProcessAsync<List<BusinessChatLink>>(new GetBusinessChatLinksQuery(userId), CancellationToken.None);
        
        return linksResult ?? new List<BusinessChatLink>();
    }

    public async Task DeleteBusinessChatLinkAsync(long userId, string linkId)
    {
        logger.LogInformation("Deleting business chat link {LinkId} for user {UserId}", linkId, userId);

        var command = new DeleteBusinessChatLinkCommand(
            UserId.Create(userId),
            linkId);

        await commandBus.PublishAsync(command, CancellationToken.None);
    }

    public async Task<bool> ShouldSendGreetingMessageAsync(long userId, long peerId, DateTime lastActivity)
    {
        var businessConfig = await GetBusinessConfigAsync(userId);
        if (businessConfig.GreetingMessage == null)
        {
            return false;
        }

        var greeting = businessConfig.GreetingMessage;
        var daysSinceLastActivity = (DateTime.UtcNow - lastActivity).Days;

        // Check if enough time has passed
        if (daysSinceLastActivity < greeting.NoActivityDays)
        {
            return false;
        }

        // Check recipient rules
        return await CheckRecipientRules(userId, peerId, greeting.Recipients);
    }

    public async Task<bool> ShouldSendAwayMessageAsync(long userId, DateTime currentTime)
    {
        var businessConfig = await GetBusinessConfigAsync(userId);
        if (businessConfig.AwayMessage == null)
        {
            return false;
        }

        var awayMessage = businessConfig.AwayMessage;
        
        switch (awayMessage.Schedule.Type)
        {
            case BusinessAwayMessageScheduleType.Always:
                return true;
                
            case BusinessAwayMessageScheduleType.OutsideWorkHours:
                if (businessConfig.WorkHours == null)
                    return true; // No work hours set, always send
                    
                var openingHours = await GetCurrentBusinessHoursAsync(userId, currentTime);
                return !openingHours.IsOpen;
                
            case BusinessAwayMessageScheduleType.Custom:
                var currentMinutes = currentTime.TimeOfDay.TotalMinutes;
                return currentMinutes >= awayMessage.Schedule.StartMinute && 
                       currentMinutes <= awayMessage.Schedule.EndMinute;
                
            default:
                return false;
        }
    }

    public async Task<BusinessOpeningHours> GetCurrentBusinessHoursAsync(long userId, DateTime currentTime)
    {
        var businessConfig = await GetBusinessConfigAsync(userId);
        if (businessConfig.WorkHours == null)
        {
            return new BusinessOpeningHours
            {
                IsOpen = false,
                TimezoneName = "UTC"
            };
        }

        var workHours = businessConfig.WorkHours;
        var dayOfWeek = (int)currentTime.DayOfWeek;
        
        // Get today's opening hours
        var todayHours = workHours.WeeklyOpen
            .Where(wo => IsDayInInterval(dayOfWeek, wo.StartMinute, wo.EndMinute))
            .ToList();

        var currentMinutes = currentTime.TimeOfDay.TotalMinutes;
        var isOpen = todayHours.Any(wh => currentMinutes >= wh.StartMinute && currentMinutes <= wh.EndMinute);

        return new BusinessOpeningHours
        {
            IsOpen = isOpen,
            TimezoneName = workHours.TimezoneId,
            TodayHours = todayHours,
            NextOpeningTime = CalculateNextOpeningTime(workHours, currentTime),
            NextClosingTime = CalculateNextClosingTime(workHours, currentTime)
        };
    }

    private async Task<bool> CheckRecipientRules(long userId, long peerId, BusinessRecipients recipients)
    {
        // Get peer information to check if they're a contact
        var contactReadModels = await queryProcessor.ProcessAsync(new GetContactListBySelfIdAndTargetUserIdQuery(userId, peerId));
        var contactType = await _contactAppService.GetContactTypeAsync(userId, peerId);
        
        bool shouldSend = false;

        if (recipients.ExistingChats && contactType != MyTelegram.ContactType.None)
            shouldSend = true;
            
        if (recipients.NewChats && contactType == MyTelegram.ContactType.None)
            shouldSend = true;
            
        if (recipients.Contacts && (contactType == MyTelegram.ContactType.Mutual || contactType == MyTelegram.ContactType.TargetUserIsMyContact))
            shouldSend = true;
            
        if (recipients.NonContacts && contactType == MyTelegram.ContactType.None)
            shouldSend = true;

        if (recipients.ExcludeSelected)
        {
            // Don't send if peer is in excluded list
            return shouldSend && !recipients.Users.Contains(peerId);
        }
        else
        {
            // Only send if peer is in selected list
            return shouldSend && (recipients.Users.Count == 0 || recipients.Users.Contains(peerId));
        }
    }

    private bool IsDayInInterval(int dayOfWeek, int startMinute, int endMinute)
    {
        var dayStart = dayOfWeek * 24 * 60;
        var dayEnd = dayStart + 24 * 60;
        
        return startMinute < dayEnd && endMinute > dayStart;
    }

    private DateTime? CalculateNextOpeningTime(BusinessWorkHours workHours, DateTime currentTime)
    {
        // Implementation for calculating next opening time
        return null; // Placeholder
    }

    private DateTime? CalculateNextClosingTime(BusinessWorkHours workHours, DateTime currentTime)
    {
        // Implementation for calculating next closing time
        return null; // Placeholder
    }

    private string GenerateLinkId()
    {
        return Guid.NewGuid().ToString("N")[..8];
    }

    // Mapping methods between domain and schema types
    private BusinessWorkHours MapToDomain(IBusinessWorkHours schema)
    {
        return new BusinessWorkHours
        {
            TimezoneId = schema.TimezoneId,
            OpenNow = schema.OpenNow,
            WeeklyOpen = schema.WeeklyOpen?.Select(wo => new BusinessWeeklyOpen
            {
                StartMinute = wo.StartMinute,
                EndMinute = wo.EndMinute
            }).ToList() ?? new List<BusinessWeeklyOpen>()
        };
    }

    private BusinessLocation MapToDomain(IBusinessLocation schema)
    {
        return new BusinessLocation
        {
            Address = schema.Address,
            Latitude = schema.GeoPoint is TGeoPoint geoPoint ? (float?)geoPoint.Lat : null,
            Longitude = schema.GeoPoint is TGeoPoint geoPoint2 ? (float?)geoPoint2.Long : null
        };
    }

    private BusinessGreetingMessage MapToDomain(IBusinessGreetingMessage schema)
    {
        return new BusinessGreetingMessage
        {
            ShortcutId = schema.ShortcutId,
            NoActivityDays = schema.NoActivityDays,
            Recipients = schema.Recipients != null ? MapToDomain(schema.Recipients) : new BusinessRecipients()
        };
    }

    private BusinessIntro MapToDomain(IBusinessIntro schema)
    {
        return new BusinessIntro
        {
            Title = schema.Title,
            Description = schema.Description,
            StickerDocumentId = schema.Sticker?.Id.ToString()
        };
    }

    private BusinessRecipients MapToDomain(IBusinessRecipients schema)
    {
        return new BusinessRecipients
        {
            ExistingChats = schema.ExistingChats,
            NewChats = schema.NewChats,
            Contacts = schema.Contacts,
            NonContacts = schema.NonContacts,
            ExcludeSelected = schema.ExcludeSelected,
            Users = schema.Users?.ToList() ?? new List<long>()
        };
    }

    private BusinessChatLink MapToDomain(IBusinessChatLink schema)
    {
        return new BusinessChatLink
        {
            Link = schema.Link,
            Title = schema.Title,
            Message = schema.Message,
            Views = schema.Views,
            Entities = schema.Entities?.Select(e => new MyTelegram.Domain.Shared.Business.MessageEntity
            {
                Type = e.GetType().Name,
                Offset = 0, // Default values
                Length = 0
            }).ToList() ?? new List<MyTelegram.Domain.Shared.Business.MessageEntity>()
        };
    }

    private BusinessAwayMessage MapToDomain(IBusinessAwayMessage schema)
    {
        return new BusinessAwayMessage
        {
            ShortcutId = schema.ShortcutId,
            Recipients = schema.Recipients != null ? MapToDomain(schema.Recipients) : new BusinessRecipients(),
            Schedule = schema.Schedule != null ? MapToDomain(schema.Schedule) : new BusinessAwayMessageSchedule(),
            OfflineOnly = schema.OfflineOnly
        };
    }

    private BusinessAwayMessageSchedule MapToDomain(IBusinessAwayMessageSchedule schema)
    {
        // Default implementation - should be extended based on actual type
        return new BusinessAwayMessageSchedule
        {
            Type = BusinessAwayMessageScheduleType.Always, // Default type
            StartMinute = null,
            EndMinute = null
        };
    }
}
