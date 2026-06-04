using MyTelegram.Domain.Shared.Forums;

namespace MyTelegram.Messenger.Services;

// Request classes
public class CreateMonoforumRequest
{
    public long ChannelId { get; set; }
    public long CreatorId { get; set; }
    public string? Title { get; set; }
    public MonoforumSettings? Settings { get; set; }
    public string? Description { get; set; }
}

public class TopicValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
}

public class MessageValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
}
