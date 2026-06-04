namespace MyTelegram.Services.Services.IdGenerator;

public interface IHiLoHighValueGenerator
{
    Task<long> GetNewHighValueAsync(IdType idType,
        long key,
        CancellationToken cancellationToken = default);
}