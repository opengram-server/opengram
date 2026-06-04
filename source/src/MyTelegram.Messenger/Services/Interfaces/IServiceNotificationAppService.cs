using MyTelegram.Schema;

namespace MyTelegram.Messenger.Services.Interfaces;

public interface IServiceNotificationAppService
{
    /// <summary>
    /// Send a service notification to a specific user
    /// </summary>
    Task SendServiceNotificationAsync(
        long userId,
        string type,
        string message,
        bool popup,
        IMessageMedia? media = null,
        List<IMessageEntity>? entities = null,
        bool invertMedia = false);

    /// <summary>
    /// Send a service notification to multiple users
    /// </summary>
    Task SendServiceNotificationToUsersAsync(
        List<long> userIds,
        string type,
        string message,
        bool popup,
        IMessageMedia? media = null,
        List<IMessageEntity>? entities = null,
        bool invertMedia = false);

    /// <summary>
    /// Send a service notification to all users (broadcast)
    /// </summary>
    Task SendServiceNotificationToAllUsersAsync(
        string type,
        string message,
        bool popup,
        IMessageMedia? media = null,
        List<IMessageEntity>? entities = null,
        bool invertMedia = false);
}
