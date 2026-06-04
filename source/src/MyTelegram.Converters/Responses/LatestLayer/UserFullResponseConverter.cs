namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class UserFullResponseConverter : IUserFullResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IUserFull ToLayeredData(TUserFull obj)
    {
        return obj;
    }
}