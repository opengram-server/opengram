namespace MyTelegram;

public class PeerNotifySettings(
    bool? showPreviews,
    bool? silent,
    int? muteUntil,
    string? sound)
{
    public static PeerNotifySettings DefaultSettings { get; } = new(true, false, 0, "default");
    public int? MuteUntil { get; init; } = muteUntil; //= 0;// = int.MaxValue;

    public bool? ShowPreviews { get; init; } = showPreviews; //= true;
    public bool? Silent { get; init; } = silent; //= false;
    public string? Sound { get; init; } = sound; //= "default";
}