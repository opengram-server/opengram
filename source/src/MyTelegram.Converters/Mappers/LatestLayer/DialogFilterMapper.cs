namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class DialogFilterMapper
    : IObjectMapper<DialogFilter, TDialogFilter>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;
    

    public TDialogFilter Map(DialogFilter source)
    {
        return Map(source, new TDialogFilter());
    }

    public TDialogFilter Map(
        DialogFilter source,
        TDialogFilter destination
    )
    {
        destination.Contacts = source.Contacts;
        destination.NonContacts = source.NonContacts;
        destination.Groups = source.Groups;
        destination.Broadcasts = source.Broadcasts;
        destination.Bots = source.Bots;
        destination.ExcludeMuted = source.ExcludeMuted;
        destination.ExcludeRead = source.ExcludeRead;
        destination.ExcludeArchived = source.ExcludeArchived;
        destination.TitleNoanimate = source.TitleNoAnimate;
        destination.Id = source.Id;
        destination.Title = source.Title;
        destination.Emoticon = source.Emoticon;
        destination.Color = source.Color;
        //destination.PinnedPeers = source.PinnedPeers;
        //destination.IncludePeers = source.IncludePeers;
        //destination.ExcludePeers = source.ExcludePeers;

        destination.PinnedPeers = [];
        destination.ExcludePeers = [];
        destination.IncludePeers = [];

        foreach (var peer in source.PinnedPeers)
        {
            destination.PinnedPeers.Add(peer.ToInputPeer());
        }

        foreach (var peer in source.ExcludePeers)
        {
            destination.ExcludePeers.Add(peer.ToInputPeer());
        }

        foreach (var peer in source.IncludePeers)
        {
            destination.IncludePeers.Add(peer.ToInputPeer());
        }

        return destination;
    }
}