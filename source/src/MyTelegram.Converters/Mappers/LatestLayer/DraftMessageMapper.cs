namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class DraftMessageMapper
    : IObjectMapper<IDraftReadModel, TDraftMessage>,
        IObjectMapper<Draft, TDraftMessage>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;


    public TDraftMessage Map(IDraftReadModel source)
    {
        return Map(source, new TDraftMessage());
    }

    public TDraftMessage Map(
        IDraftReadModel source,
        TDraftMessage destination
    )
    {
        return Map(source.Draft, destination);
    }

    public TDraftMessage Map(Draft source)
    {
        return Map(source, new TDraftMessage());
    }

    public TDraftMessage Map(Draft source, TDraftMessage destination)
    {
        destination.NoWebpage = source.NoWebpage;
        destination.InvertMedia = source.InvertMedia;
        //destination.ReplyTo = source.ReplyToMsgId;

        if (source.ReplyToMsgId != null)
        {
            destination.ReplyTo = new TInputReplyToMessage
            {
                ReplyToMsgId = source.ReplyToMsgId.Value
            };
        }

        if (source.ReplyTo != null)
        {
            destination.ReplyTo = source.ReplyTo;
        }

        destination.Media = source.Media2;

        destination.Message = source.Message;
        //destination.Entities = source.Entities;
        destination.Entities = source.Entities2?.Count > 0
            ? new TVector<IMessageEntity>(source.Entities2)
            : source.Entities.ToTObject<TVector<IMessageEntity>>();

        //destination.Media = source.Media;

        destination.Date = source.Date;
        destination.Effect = source.Effect;

        return destination;
    }
}