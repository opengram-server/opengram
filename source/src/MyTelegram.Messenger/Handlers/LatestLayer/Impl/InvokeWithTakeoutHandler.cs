// ReSharper disable All

namespace MyTelegram.Handlers;

///<summary>
/// Invoke a method within a takeout session
/// See <a href="https://corefork.telegram.org/method/invokeWithTakeout" />
///</summary>
internal sealed class InvokeWithTakeoutHandler : BaseObjectHandler<MyTelegram.Schema.RequestInvokeWithTakeout, IObject>,
    IInvokeWithTakeoutHandler
{
    private readonly IHandlerHelper _handlerHelper;

    public InvokeWithTakeoutHandler(IHandlerHelper handlerHelper)
    {
        _handlerHelper = handlerHelper;
    }

    protected override Task<IObject> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.RequestInvokeWithTakeout obj)
    {
        // Get the handler for the inner query
        if (_handlerHelper.TryGetHandler(obj.Query.ConstructorId, out var handler))
        {
            // Forward the inner query to the appropriate handler
            // The takeoutId (obj.TakeoutId) could be used for validation or logging if needed
            return handler.HandleAsync(input, obj.Query)!;
        }

        throw new NotImplementedException($"Handler not found for query: {obj.Query.ConstructorId:x2}");
    }
}
