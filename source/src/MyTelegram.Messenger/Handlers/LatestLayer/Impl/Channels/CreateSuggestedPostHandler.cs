using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyTelegram.Handlers;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Schema.Channels;
using MyTelegram.Services.Services;
using MyTelegram.Schema;
using MyTelegram.Domain.Shared.SuggestedPosts;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Channels;

public class CreateSuggestedPostHandler : BaseObjectHandler<RequestCreateSuggestedPost, IObject>, ICreateSuggestedPostHandler
{
    private readonly ILogger<CreateSuggestedPostHandler> _logger;
    private readonly ISuggestedPostAppService _suggestedPostAppService;

    public CreateSuggestedPostHandler(
        ILogger<CreateSuggestedPostHandler> logger,
        ISuggestedPostAppService suggestedPostAppService)
    {
        _logger = logger;
        _suggestedPostAppService = suggestedPostAppService;
    }

    protected override async Task<IObject> HandleCoreAsync(IRequestInput input, RequestCreateSuggestedPost obj)
    {
        _logger.LogInformation("Creating suggested post for channel {ChannelId}", obj.ChannelId);

        try
        {
            var createRequest = new CreateSuggestedPostRequest
            {
                ChannelId = obj.ChannelId,
                SuggestedBy = input.UserId,
                Content = obj.Message ?? string.Empty,
                Type = SuggestedPostType.Text
            };
            
            var result = await _suggestedPostAppService.CreateSuggestedPostAsync(createRequest);
            
            return new TMessage
            {
                Id = (int)long.Parse(result.PostId),
                Message = "Suggested post created successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating suggested post");
            throw;
        }
    }
}

public interface ICreateSuggestedPostHandler
{
}
