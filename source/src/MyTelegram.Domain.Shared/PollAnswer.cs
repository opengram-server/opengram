namespace MyTelegram;

public record PollAnswer(string Text,
    string Option, byte[]? Entities);