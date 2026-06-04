using MyTelegram.Schema.Payments;

namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IBotConverter : ILayeredConverter
{
    IBotInfo ToBotInfo(IBotReadModel botReadModel);
}
