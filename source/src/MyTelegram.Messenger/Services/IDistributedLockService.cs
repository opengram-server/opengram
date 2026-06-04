namespace MyTelegram.Messenger.Services;

/// <summary>
/// Интерфейс сервиса распределённой блокировки.
/// Защищает от гонок в критичных операциях (например, апгрейд подарков, платежи)
/// </summary>
public interface IDistributedLockService
{
    /// <summary>
    /// Захватывает распределённую блокировку с автоматическим освобождением
    /// </summary>
    Task<IAsyncDisposable?> AcquireLockAsync(string key, TimeSpan timeout, CancellationToken cancellationToken = default);
}

public class InMemoryDistributedLockService : IDistributedLockService
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
    private readonly ILogger<InMemoryDistributedLockService> _logger;

    public InMemoryDistributedLockService(ILogger<InMemoryDistributedLockService> logger)
    {
        _logger = logger;
    }

    public async Task<IAsyncDisposable?> AcquireLockAsync(string key, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        
        var acquired = await semaphore.WaitAsync(timeout, cancellationToken);
        
        if (!acquired)
        {
            _logger.LogWarning("Failed to acquire lock for key: {Key}", key);
            return null;
        }

        _logger.LogDebug("Lock acquired for key: {Key}", key);
        
        return new LockReleaser(key, semaphore, _logger);
    }
    
    private class LockReleaser : IAsyncDisposable
    {
        private readonly string _key;
        private readonly SemaphoreSlim _semaphore;
        private readonly ILogger _logger;
        private bool _disposed;

        public LockReleaser(string key, SemaphoreSlim semaphore, ILogger logger)
        {
            _key = key;
            _semaphore = semaphore;
            _logger = logger;
        }

        public ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                _semaphore.Release();
                _logger.LogDebug("Lock released for key: {Key}", _key);
                _disposed = true;
            }
            return ValueTask.CompletedTask;
        }
    }
}
