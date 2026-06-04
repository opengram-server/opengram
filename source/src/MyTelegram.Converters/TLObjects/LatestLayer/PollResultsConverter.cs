using System.Text;

namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class PollResultsConverter(IObjectMapper objectMapper) : IPollResultsConverter, ITransientDependency
{
    
    public int Layer => Layers.LayerLatest;

    public IPollResults ToPollResults(IPollReadModel pollReadModel, IList<string>? chosenOptions)
    {
        var pollResults = objectMapper.Map<IPollReadModel, TPollResults>(pollReadModel);
        chosenOptions ??= [];
        if (pollReadModel.AnswerVoters != null)
        {
            var voters = pollReadModel.AnswerVoters.Select(p => new TPollAnswerVoters
            {
                Correct = p.Correct,
                Voters = p.Voters,
                Option = Encoding.UTF8.GetBytes(p.Option),
                Chosen = chosenOptions.Contains(p.Option)
            });
            pollResults.Results = new TVector<IPollAnswerVoters>(voters);
        }
        else
        {
            var voters = pollReadModel.Answers.Select(p => new TPollAnswerVoters
            {
                Correct = false,
                Voters = 0,
                Option = Encoding.UTF8.GetBytes(p.Option),
                Chosen = chosenOptions.Contains(p.Option)
            });
            pollResults.Results = new TVector<IPollAnswerVoters>(voters);
        }

        return pollResults;
    }
}