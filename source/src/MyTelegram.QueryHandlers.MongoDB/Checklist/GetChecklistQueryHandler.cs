using MyTelegram.EventFlow.ReadStores;
using MyTelegram.ReadModel.Impl;
using MyTelegram.Queries;

namespace MyTelegram.QueryHandlers.MongoDB.Checklist;

public class GetChecklistQueryHandler : IQueryHandler<GetChecklistQuery, MyTelegram.Domain.Shared.Checklists.Checklist?>
{
    private readonly IQueryOnlyReadModelStore<ChecklistReadModel> _store;

    public GetChecklistQueryHandler(IQueryOnlyReadModelStore<ChecklistReadModel> store)
    {
        _store = store;
    }

    public async Task<MyTelegram.Domain.Shared.Checklists.Checklist?> ExecuteQueryAsync(GetChecklistQuery query, CancellationToken cancellationToken)
    {
        var readModel = await _store.FirstOrDefaultAsync(r => r.Id == query.ChecklistId, cancellationToken);
        if (readModel == null) return null;

        return new MyTelegram.Domain.Shared.Checklists.Checklist
        {
            Id = readModel.Id,
            SenderId = readModel.SenderId,
            Title = readModel.Title,
            Tasks = readModel.Tasks,
            // Map other fields as needed
        };
    }
}
