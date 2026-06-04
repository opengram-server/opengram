namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IPollConverter : ILayeredConverter
{
    IPoll ToPoll(IPollReadModel readModel);
    IPollResults ToPollResults(IPollReadModel pollReadModel, IList<string>? chosenOptions);
}