namespace MyTelegram;

public static class TestLogExtensions
{
    public static void WriteDebugMessageToConsole(this string message)
    {
#if DEBUG
        var fColor = Console.ForegroundColor;
        var bColor = Console.BackgroundColor;

        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine(message);
        Console.ForegroundColor = fColor;
        Console.BackgroundColor = bColor;
#endif
    }

    public static ulong ToUInt64(this long value)
    {
        return BitConverter.ToUInt64(BitConverter.GetBytes(value));
    }

}