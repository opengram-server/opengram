using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyTelegram.Handlers;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Schema.Messages;
using MyTelegram.Services.Services;
using MyTelegram.Schema;
using MyTelegram.Domain.Shared.DirectMessages;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

public class CreateDirectMessageHandler : BaseObjectHandler<RequestCreateDirectMessage, IObject>, ICreateDirectMessageHandler
{
    private readonly ILogger<CreateDirectMessageHandler> _logger;
    private readonly IDirectMessageAppService _directMessageAppService;

    public CreateDirectMessageHandler(
        ILogger<CreateDirectMessageHandler> logger,
        IDirectMessageAppService directMessageAppService)
    {
        _logger = logger;
        _directMessageAppService = directMessageAppService;
    }

    protected override async Task<IObject> HandleCoreAsync(IRequestInput input, RequestCreateDirectMessage obj)
    {
        _logger.LogInformation("Creating direct message to {UserId}", obj.UserId);

        try
        {
            var createRequest = new CreateDirectMessageRequest
            {
                TopicId = Guid.NewGuid().ToString(), // Generate new topic ID
                SenderId = input.UserId,
                ChannelId = obj.UserId,
                Content = obj.Message,
                Type = DirectMessageType.Text
            };
            
            var result = await _directMessageAppService.SendMessageAsync(createRequest);
            
            return new TMessage
            {
                Id = (int)result.MessageId,
                Message = "Direct message sent successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating direct message");
            throw;
        }
    }
}

public interface ICreateDirectMessageHandler
{
}
