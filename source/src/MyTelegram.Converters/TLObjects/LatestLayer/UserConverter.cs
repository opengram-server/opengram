namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class UserConverter(IObjectMapper objectMapper)
    : LayeredConverterBase, IUserConverter, ITransientDependency
{
    public override int Layer => Layers.LayerLatest;

    public ILayeredUser ToUser(IUserReadModel userReadModel)
    {
        return objectMapper.Map<IUserReadModel, TUser>(userReadModel);
    }

    public List<ILayeredUser> ToUserList(IEnumerable<IUserReadModel> userReadModels)
    {
        return userReadModels.Select(p => ToUser(p)).ToList();
    }
}