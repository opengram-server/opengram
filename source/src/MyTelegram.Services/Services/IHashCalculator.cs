namespace MyTelegram.Services.Services;

public interface IHashCalculator
{
    long GetHash(IEnumerable<long> ids);
}