namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IBotInfoConverter : ILayeredConverter
{
    IBotInfo ToBotInfo(IBotReadModel botReadModel);
}