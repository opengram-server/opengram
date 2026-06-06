namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl;

///<summary>
/// Initialize connection and save information on the user's device and application.
/// See <a href="https://corefork.telegram.org/method/initConnection" />
///</summary>
internal sealed class InitConnectionHandler(
    IHandlerHelper handlerHelper,
    IEventBus eventBus)
    : RpcResultObjectHandler<MyTelegram.Schema.RequestInitConnection, IObject>,
        IInitConnectionHandler
{
    protected override async Task<IObject> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.RequestInitConnection obj)
    {
        // Save device info for the connection
        if (input.PermAuthKeyId != 0)
        {
            var eventData = new NewDeviceCreatedEvent(
                input.ToRequestInfo(),
                input.PermAuthKeyId,
                input.AuthKeyId,
                input.UserId,
                obj.ApiId,
                obj.AppVersion,
                obj.AppVersion,
                0,
                false,
                false,
                obj.DeviceModel,
                obj.SystemVersion,
                obj.SystemVersion,
                obj.SystemLangCode,
                obj.LangPack,
                obj.LangCode,
                input.ClientIp,
                input.Layer,
                null);
            await eventBus.PublishAsync(eventData);
        }

        // Delegate to the inner query
        if (!handlerHelper.TryGetHandler(obj.Query.ConstructorId, out var handler))
        {
            throw new RpcException(new RpcError(400, "INPUT_METHOD_INVALID"));
        }

        return await handler.HandleAsync(input, obj.Query);
    }
}
