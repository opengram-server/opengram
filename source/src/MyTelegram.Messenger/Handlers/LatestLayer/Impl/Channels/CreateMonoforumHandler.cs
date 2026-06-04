using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyTelegram.Handlers;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Schema.Channels;
using MyTelegram.Services.Services;
using MyTelegram.Schema;
using MyTelegram.Domain.Shared.Forums;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Channels;

public sealed class CreateMonoforumHandler : BaseObjectHandler<RequestCreateMonoforum, IObject>, ICreateMonoforumHandler
{
    private readonly ILogger<CreateMonoforumHandler> _logger;
    private readonly IMonoforumAppService _monoforumAppService;

    public CreateMonoforumHandler(
        ILogger<CreateMonoforumHandler> logger,
        IMonoforumAppService monoforumAppService)
    {
        _logger = logger;
        _monoforumAppService = monoforumAppService;
    }

    protected override async Task<IObject> HandleCoreAsync(IRequestInput input, RequestCreateMonoforum obj)
    {
        _logger.LogInformation("Creating monoforum for channel {ChannelId}", obj.ChannelId);

        try
        {
            var createRequest = new CreateMonoforumRequest
            {
                ChannelId = obj.ChannelId,
                CreatorId = input.UserId
            };
            
            var result = await _monoforumAppService.CreateMonoforumAsync(createRequest);
            
            return new TChannel
            {
                Id = obj.ChannelId,
                Title = "Monoforum"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating monoforum");
            throw;
        }
    }
}

public interface ICreateMonoforumHandler
{
}
