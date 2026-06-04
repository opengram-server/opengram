namespace MyTelegram.ReadModel.Impl;

public class ServiceNotificationTemplateReadModel : IServiceNotificationTemplateReadModel
{
    public ServiceNotificationTemplateReadModel(
        string id,
        string type,
        string title,
        string message,
        string? mediaUrl,
        string? mediaType,
        bool isPopup,
        bool isActive,
        int createdDate,
        int? updatedDate)
    {
        Id = id;
        Type = type;
        Title = title;
        Message = message;
        MediaUrl = mediaUrl;
        MediaType = mediaType;
        IsPopup = isPopup;
        IsActive = isActive;
        CreatedDate = createdDate;
        UpdatedDate = updatedDate;
    }

    public string Id { get; private set; }
    public string Type { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public string? MediaUrl { get; private set; }
    public string? MediaType { get; private set; }
    public bool IsPopup { get; private set; }
    public bool IsActive { get; private set; }
    public int CreatedDate { get; private set; }
    public int? UpdatedDate { get; private set; }
}
