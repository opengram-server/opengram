namespace MyTelegram.Messenger.Converters.ConverterServices;

public class PollConverterService(
    ILayeredService<IPollConverter> pollLayeredService,
    ILayeredService<IPollResultsConverter> pollResultsLayeredService) : IPollConverterService, ITransientDependency
{
    public IPoll ToPoll(IPollReadModel pollReadModel, int layer = 0)
    {
        return pollLayeredService.GetConverter(layer).ToPoll(pollReadModel);
    }

    public IPollResults ToPollResults(IPollReadModel pollReadModel, IList<string> chosenOptions, int layer = 0)
    {
        return pollResultsLayeredService.GetConverter(layer).ToPollResults(pollReadModel, chosenOptions);
    }

    public IUpdates ToPollUpdates(IPollReadModel pollReadModel, IList<string> chosenOptions, int layer = 0)
    {
        var pollResults = ToPollResults(pollReadModel, chosenOptions);
        pollResults.Min = true;

        var updateMessagePoll = new TUpdateMessagePoll
        {
            PollId = pollReadModel.PollId,
            Results = pollResults
        };

        return new TUpdateShort
        {
            Date = DateTime.UtcNow.ToTimestamp(),
            Update = updateMessagePoll
        };
    }
}