namespace MyTelegram;

public interface IClock
{
    /// <summary>
    ///     Gets Now.
    /// </summary>
    DateTime Now { get; }

    /// <summary>
    ///     Gets kind.
    /// </summary>
    DateTimeKind Kind { get; }
}