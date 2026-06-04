namespace MyTelegram;

public class BotCommand(
    string command,
    string description)
{
    public string Command { get; init; } = command;
    public string Description { get; init; } = description;
}