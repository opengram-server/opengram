namespace MyTelegram.Services.Services.IdGenerator;

public interface IHiLoValueGeneratorFactory
{
    HiLoValueGenerator<long> Create(HiLoValueGeneratorState state);
}