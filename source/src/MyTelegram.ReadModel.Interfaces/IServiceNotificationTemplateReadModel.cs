namespace MyTelegram.ReadModel;

public interface IServiceNotificationTemplateReadModel : IReadModel
{
    string Type { get; }
    string Title { get; }
    string Message { get; }
    string? MediaUrl { get; }
    string? MediaType { get; } // "photo" or "video"
    bool IsPopup { get; }
    bool IsActive { get; }
    int CreatedDate { get; }
    int? UpdatedDate { get; }
}
