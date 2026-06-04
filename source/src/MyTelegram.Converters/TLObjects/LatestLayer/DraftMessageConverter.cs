namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class DraftMessageConverter(IObjectMapper objectMapper) : IDraftMessageConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IDraftMessage ToDraftMessage(IDraftReadModel draftReadModel)
    {
        return objectMapper.Map<IDraftReadModel, TDraftMessage>(draftReadModel);
    }

    public IDraftMessage ToDraftMessage(Draft draft)
    {
        return objectMapper.Map<Draft, TDraftMessage>(draft);

    }
}