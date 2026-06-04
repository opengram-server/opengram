using EventFlow.Core;

namespace MyTelegram.Domain.Aggregates.Checklist;

[JsonConverter(typeof(SingleValueObjectConverter))]
public class ChecklistId : Identity<ChecklistId>
{
    public ChecklistId(string value) : base(value)
    {
    }
}
