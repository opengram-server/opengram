namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class PollMapper
    : IObjectMapper<IPollReadModel, TPoll>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;
    

    public TPoll Map(IPollReadModel source)
    {
        return Map(source, new TPoll());
    }

    public TPoll Map(
        IPollReadModel source,
        TPoll destination
    )
    {
        destination.Id = source.PollId;
        destination.Closed = source.Closed;
        destination.PublicVoters = source.PublicVoters;
        destination.MultipleChoice = source.MultipleChoice;
        destination.Quiz = source.Quiz;
        destination.Question = new TTextWithEntities
        {
            Text = source.Question,
            Entities = source.QuestionEntities2 != null
                ? [.. source.QuestionEntities2]
                : source.QuestionEntities.ToTObject<TVector<IMessageEntity>>() ?? []
        };
        destination.Answers = new TVector<IPollAnswer>(source.Answers.Select(p => new TPollAnswer
        {
            Option = p.Option,
            Text = new TTextWithEntities
            {
                Text = p.Text,
                Entities = p.Entities.ToTObject<TVector<IMessageEntity>>() ?? []
            }
        }));
        destination.ClosePeriod = source.ClosePeriod;
        destination.CloseDate = source.CloseDate;

        return destination;
    }
}