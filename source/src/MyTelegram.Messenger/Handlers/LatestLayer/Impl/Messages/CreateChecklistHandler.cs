using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Sagas;
using MyTelegram.Handlers;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Schema.Messages;
using MyTelegram.Services.Services;
using MyTelegram.Schema;
using MyTelegram.Domain.Shared.Checklists;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

public class CreateChecklistHandler : BaseObjectHandler<RequestCreateChecklist, IObject>, ICreateChecklistHandler
{
    private readonly ILogger<CreateChecklistHandler> _logger;
    private readonly IChecklistAppService _checklistAppService;
    private readonly IPeerHelper _peerHelper;

    public CreateChecklistHandler(
        ILogger<CreateChecklistHandler> logger,
        IChecklistAppService checklistAppService,
        IPeerHelper peerHelper)
    {
        _logger = logger;
        _checklistAppService = checklistAppService;
        _peerHelper = peerHelper;
    }

    protected override async Task<IObject> HandleCoreAsync(IRequestInput input, RequestCreateChecklist obj)
    {
        _logger.LogInformation("Creating checklist for peer {PeerId}", obj.Peer);

        try
        {
            // Create checklist request
            var checklist = new MyTelegram.Domain.Shared.Checklists.InputChecklist
            {
                Title = obj.Title,
                Tasks = obj.Tasks.Select(t => new MyTelegram.Domain.Shared.Checklists.InputChecklistTask
                {
                    Text = t.Text,
                    //Entities = t.Entities.Select(e => e.ToString()).ToList(), // Simplified mapping
                    IsMandatory = t.IsMandatory
                }).ToList(),
                //TitleEntities = obj.TitleEntities.Select(e => e.ToString()).ToList() // Simplified mapping
            };

            var request = new CreateChecklistRequest
            {
                SenderId = input.UserId,
                PeerId = _peerHelper.GetPeer(obj.Peer).PeerId, 
                Checklist = checklist
            };

            var result = await _checklistAppService.CreateChecklistAsync(request);
            
            // Return Updates object as expected by MTProto
            return new TUpdates
            {
                Updates = new TVector<IUpdate>(),
                Users = new TVector<IUser>(),
                Chats = new TVector<IChat>(),
                Date = (int)DateTime.UtcNow.ToTimestamp()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating checklist");
            throw;
        }
    }
}

public interface ICreateChecklistHandler
{
}
