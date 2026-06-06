using System.Diagnostics;

namespace MyTelegram.SessionServer.Services;

/// <summary>
/// Per-session rate limiter. Reconstructed from the original binary
/// (methods at 0x00a0dad0..0x00a0db40).
/// Uses <see cref="Stopwatch.GetTimestamp"/> as a monotonic nanosecond clock.
/// </summary>
public sealed class TokenBucket
{
    private int _tokens;
    private long _lastRefillTimestamp;
    private readonly int _tokenRate;

    public TokenBucket(int capacity, int tokenRate)
    {
        _tokenRate = tokenRate;
        _tokens = capacity;
        _lastRefillTimestamp = Stopwatch.GetTimestamp();
    }

    public bool TryConsume()
    {
        RefillTokens();
        if (_tokens < 1) return false;
        Interlocked.Decrement(ref _tokens);
        return true;
    }

    private void RefillTokens()
    {
        var now = Stopwatch.GetTimestamp();
        var elapsedTicks = now - _lastRefillTimestamp;
        if (elapsedTicks <= 0) return;

        var elapsedSeconds = (double)elapsedTicks / Stopwatch.Frequency;
        var produced = elapsedSeconds * _tokenRate;
        var newTokens = produced < int.MaxValue ? (int)produced : int.MaxValue;
        if (newTokens <= 0) return;

        var old = _tokens;
        var sum = old + newTokens;
        // Saturating add: if overflow occurred, keep old value
        if (sum < old) sum = old;
        _tokens = sum;
        _lastRefillTimestamp = now;
    }
}
