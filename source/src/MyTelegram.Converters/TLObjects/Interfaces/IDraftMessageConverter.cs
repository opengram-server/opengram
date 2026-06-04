namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IDraftMessageConverter : ILayeredConverter
{
    IDraftMessage ToDraftMessage(IDraftReadModel draftReadModel);
    IDraftMessage ToDraftMessage(Draft draft);
}