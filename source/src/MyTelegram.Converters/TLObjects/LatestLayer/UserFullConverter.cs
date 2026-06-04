namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class UserFullConverter(IObjectMapper objectMapper) : IUserFullConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IUserFull ToUserFull(IUserReadModel userReadModel)
    {
        return objectMapper.Map<IUserReadModel, TUserFull>(userReadModel);
    }
}