namespace MyTelegram.Abstractions;

public interface IMessageQueueProcessor<in TData>
{
    void Enqueue(TData message,
        long key);

    Task ProcessAsync(CancellationToken cancellationToken = default);
}