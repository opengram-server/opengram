namespace MyTelegram.Core;

public class RandomHelper : IRandomHelper, ISingletonDependency
{
    private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const string NumberCharacters = "0123456789";

    public string GenerateRandomNumber(int length)
    {
        return new string(Enumerable.Repeat(NumberCharacters, length)
            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
    }

    public string GenerateRandomString(int length)
    {
        return new string(Enumerable.Repeat(Characters, length)
            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
    }

    public void NextBytes(byte[] buffer)
    {
        Random.Shared.NextBytes(buffer);
    }

    public byte[] NextBytes(int length)
    {
        var buffer = new byte[length];
        Random.Shared.NextBytes(buffer);

        return buffer;
    }

    public int NextInt(int min,
        int max)
    {
        return Random.Shared.Next(min, max);
    }

    public int NextInt()
    {
        return Random.Shared.Next();
    }

    public long NextInt64()
    {
        return Random.Shared.NextInt64();
    }
}