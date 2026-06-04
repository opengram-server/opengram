namespace MyTelegram.Services.Services;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TOldLayerRequestData">Old layer request data</typeparam>
/// <typeparam name="TNewLayerRequestData">New layer request data</typeparam>
/// <param name="handlerHelper"></param>
/// <param name="requestDataConverter"></param>
public abstract class ForwardRequestToNewHandler<TOldLayerRequestData, TNewLayerRequestData>(
    IHandlerHelper handlerHelper,
    IRequestConverter<TOldLayerRequestData, TNewLayerRequestData> requestDataConverter) : BaseObjectHandler<TOldLayerRequestData, IObject>
    where TOldLayerRequestData : IRequest<IObject>
    where TNewLayerRequestData : IRequest<IObject>
{
    protected override async Task<IObject> HandleCoreAsync(IRequestInput request, TOldLayerRequestData obj)
    {
        var newRequestData = requestDataConverter.ToLatestLayerData(request, obj);
        if (newRequestData.ConstructorId == obj.ConstructorId)
        {
            throw new InvalidOperationException("The new and old handlers cannot be the same");
        }

        if (handlerHelper.TryGetHandler(newRequestData.ConstructorId, out var handler))
        {
            var result = await handler.HandleAsync(request, newRequestData);

            return result;
        }

        throw new NotImplementedException();
    }
}