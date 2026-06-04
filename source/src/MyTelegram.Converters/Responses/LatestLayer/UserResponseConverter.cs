namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class UserResponseConverter : IUserResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public ILayeredUser ToLayeredData(TUser obj)
    {
        return obj;
    }
}