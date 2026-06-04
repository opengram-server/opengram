namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class PeerNotifySettingsMapper
    : IObjectMapper<PeerNotifySettings, TPeerNotifySettings>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;
    

    public TPeerNotifySettings Map(PeerNotifySettings source)
    {
        return Map(source, new TPeerNotifySettings());
    }

    public TPeerNotifySettings Map(
        PeerNotifySettings source,
        TPeerNotifySettings destination
    )
    {
        destination.ShowPreviews = source.ShowPreviews;
        destination.Silent = source.Silent;
        destination.MuteUntil = source.MuteUntil;
        //destination.IosSound = source.IosSound;
        //destination.AndroidSound = source.AndroidSound;
        //destination.OtherSound = source.OtherSound;
        //destination.StoriesMuted = source.StoriesMuted;
        //destination.StoriesHideSender = source.StoriesHideSender;
        //destination.StoriesIosSound = source.StoriesIosSound;
        //destination.StoriesAndroidSound = source.StoriesAndroidSound;
        //destination.StoriesOtherSound = source.StoriesOtherSound;

        return destination;
    }
}