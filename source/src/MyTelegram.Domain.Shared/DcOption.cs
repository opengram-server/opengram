namespace MyTelegram;

public class DcOption
{
    public bool Enabled { get; set; }
    public bool Ipv6 { get; set; }
    public bool MediaOnly { get; set; }
    public bool TcpoOnly { get; set; }
    public bool ThisPortOnly { get; set; }
    public bool Cdn { get; set; }
    public bool Static { get; set; }
    public int Id { get; set; }
    public int Port { get; set; }
    public byte[]? Secret { get; set; }
    public string IpAddress { get; set; } = default!;
}