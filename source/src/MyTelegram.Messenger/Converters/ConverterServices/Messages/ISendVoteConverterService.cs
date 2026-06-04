namespace MyTelegram.Messenger.Converters.ConverterServices.Messages;

public interface ISendVoteConverterService
{
    IUpdates ToSelfUpdates(IPollReadModel pollReadModel, List<string> chosenOptions, int layer);
    IUpdates ToUpdates(IPollReadModel pollReadModel, List<string> chosenOptions);
}