using RabbitMQ.Client;

namespace MyTelegram.SessionServer.Services;

public interface IMessageBusService
{
    Task PublishAsync(string exchangeName, string routingKey, object message);
    Task SubscribeAsync(string exchangeName, string queueName, string routingKey, Func<string, Task> handler);
    Task StartConsumingAsync(string queueName, Func<string, Task> handler);
}
