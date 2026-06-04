namespace MyTelegram;

public class PasswordKdfAlgoSha256Sha256Pbkdf2Hmacsha512Iter100000Sha256ModPow(
    byte[] salt1,
    byte[] salt2,
    int g,
    byte[] p)
{
    public int G { get; init; } = g;
    public byte[] P { get; init; } = p;

    public byte[] Salt1 { get; init; } = salt1;
    public byte[] Salt2 { get; init; } = salt2;
}