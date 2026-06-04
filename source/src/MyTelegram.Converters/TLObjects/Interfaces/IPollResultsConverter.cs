namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IPollResultsConverter : ILayeredConverter
{
    IPollResults ToPollResults(IPollReadModel pollReadModel, IList<string>? chosenOptions);
}