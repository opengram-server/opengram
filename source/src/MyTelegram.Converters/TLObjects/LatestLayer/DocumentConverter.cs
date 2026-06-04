namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class DocumentConverter(IObjectMapper objectMapper) : IDocumentConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public ILayeredDocument ToDocument(IDocumentReadModel documentReadModel)
    {
        return objectMapper.Map<IDocumentReadModel, TDocument>(documentReadModel);
    }
}