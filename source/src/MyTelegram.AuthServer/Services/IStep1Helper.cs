namespace MyTelegram.AuthServer.Services;

public interface IStep1Helper
{
    Step1Output GetResponse(byte[] nonce);
}