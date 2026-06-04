namespace MyTelegram.Services.Services.IdGenerator;

public interface IHiLoStateBlockSizeHelper
{
    int GetBlockSize(IdType idType);
}