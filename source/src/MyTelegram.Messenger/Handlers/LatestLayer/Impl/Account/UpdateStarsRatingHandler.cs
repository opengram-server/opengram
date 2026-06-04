using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyTelegram.Handlers;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Schema.Account;
using MyTelegram.Services.Services;
using MyTelegram.Schema;
using MyTelegram.Schema.Payments;

using MyTelegram.Domain.Shared.StarsRating;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

public class UpdateStarsRatingHandler : BaseObjectHandler<RequestUpdateStarsRating, IObject>, IUpdateStarsRatingHandler
{
    private readonly ILogger<UpdateStarsRatingHandler> _logger;
    private readonly IStarsRatingAppService _starsRatingAppService;

    public UpdateStarsRatingHandler(
        ILogger<UpdateStarsRatingHandler> logger,
        IStarsRatingAppService starsRatingAppService)
    {
        _logger = logger;
        _starsRatingAppService = starsRatingAppService;
    }

    protected override async Task<IObject> HandleCoreAsync(IRequestInput input, RequestUpdateStarsRating obj)
    {
        _logger.LogInformation("Updating stars rating for user {UserId}", input.UserId);

        try
        {
            var result = await _starsRatingAppService.UpdateRatingAsync(new UpdateStarsRatingRequest
            {
                UserId = input.UserId,
                StarsAmount = obj.Rating,
                ActivityType = MyTelegram.Domain.Shared.StarsRating.StarsActivityType.ManualUpdate,
                Comment = "" // Default empty comment
            });
            
            return new TStarsStatus
            {
                Balance = new TStarsAmount { Amount = result.TotalStars }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stars rating");
            throw;
        }
    }
}

public interface IUpdateStarsRatingHandler
{
}
