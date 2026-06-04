using MyTelegram.Messenger.Services.Impl;

namespace MyTelegram.Messenger.Services.Impl;

// Custom interfaces to avoid conflicts with EventFlow and framework
public interface IMessengerQueryProcessor
{
    Task<T?> ProcessAsync<T>(T query) where T : class;
}

public interface IMessengerCommandBus  
{
    Task PublishAsync<T>(T command) where T : class;
}

public interface IMessengerTransientDependency
{
}
