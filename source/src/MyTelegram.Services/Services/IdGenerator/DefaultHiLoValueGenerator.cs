namespace MyTelegram.Services.Services.IdGenerator;

public class DefaultHiLoValueGenerator(
    HiLoValueGeneratorState generatorState,
    ILogger<DefaultHiLoValueGenerator> logger,
    IHiLoHighValueGenerator highValueGenerator)
    : HiLoValueGenerator<long>(generatorState)
{
    private readonly int _blockSize = generatorState.BlockSize;

    protected override long GetNewLowValue(IdType idType,
        long key)
    {
        throw new NotImplementedException();
    }

    protected override async Task<long> GetNewLowValueAsync(IdType idType,
        long key,
        CancellationToken cancellationToken = default)
    {
        var highValue = await highValueGenerator.GetNewHighValueAsync(idType, key, cancellationToken)
     ;
        logger.LogInformation("Get new low value from db, idType: {IdType} key: {Key} newHighValue: {HighValue}",
            idType,
            key,
            highValue);

        return (highValue-1) * _blockSize + 1;
    }
}
