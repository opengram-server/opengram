namespace MyTelegram.Messenger.Converters.ConverterServices;

public interface IPollConverterService
{
    IPoll ToPoll(IPollReadModel pollReadModel, int layer = 0);
    IPollResults ToPollResults(IPollReadModel pollReadModel, IList<string> chosenOptions, int layer = 0);
    IUpdates ToPollUpdates(IPollReadModel pollReadModel, IList<string> chosenOptions, int layer = 0);
}