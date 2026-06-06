namespace MyTelegram.SessionServer.Services;

/// <summary>
/// Generates MTProto server-side message_id values.
/// MTProto message_id = (unixtime_in_seconds * 2^32) | (seqno * 4)
/// Message IDs must be even (bit 0 = 0) for server-originated messages.
/// </summary>
public static class MessageIdGenerator
{
    private static long _counter;

    public static long GenerateServerMessageId()
    {
        var unixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var seq = Interlocked.Increment(ref _counter) & 0x7FFFFF; // 23-bit counter
        // message_id for server messages: high 32 bits = unix time, low 32 bits = counter * 4
        // The lowest 2 bits determine message type: 0 = response to client, 1 = content-related, etc.
        return (unixSeconds << 32) | ((seq << 2) & 0xFFFFFFFC);
    }

    /// <summary>
    /// Validates a client-supplied message_id.
    /// Client message_ids must satisfy: (msg_id % 4) == 1 or 3.
    /// Additionally, msg_id should be reasonably close to server time.
    /// </summary>
    public static bool IsValidClientMessageId(long messageId)
    {
        var mod4 = messageId & 3;
        if (mod4 != 1 && mod4 != 3)
            return false;

        // Check that the timestamp portion is within ±300 seconds of now
        var msgTime = messageId >> 32;
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var diff = Math.Abs(now - msgTime);
        return diff <= 300;
    }
}
