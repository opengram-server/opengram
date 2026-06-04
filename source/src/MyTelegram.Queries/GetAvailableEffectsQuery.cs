using EventFlow.Queries;
using MyTelegram.ReadModel;

namespace MyTelegram.Queries;

public class GetAvailableEffectsQuery : IQuery<IReadOnlyCollection<IMessageEffectReadModel>>
{
    public GetAvailableEffectsQuery()
    {
    }
}
