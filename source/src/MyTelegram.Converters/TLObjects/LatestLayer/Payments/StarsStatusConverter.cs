// ReSharper disable All

using MyTelegram.Schema.Payments;
using MyTelegram.Domain.Shared.Stars;
using MyTelegram.Schema;

namespace MyTelegram.Converters.TLObjects.Payments;

internal sealed class StarsStatusConverter : IStarsStatusConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IStarsStatus ToStarsStatus(StarsStatus starsStatus)
    {
        return new TStarsStatus
        {
            Balance = new TStarsAmount { Amount = starsStatus.Balance },
            History = new TVector<IStarsTransaction>(), // Transactions converted separately
            NextOffset = starsStatus.NextOffset,
            Chats = new TVector<IChat>(),
            Users = new TVector<IUser>()
        };
    }
}
