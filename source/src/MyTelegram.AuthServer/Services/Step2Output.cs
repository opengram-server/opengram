namespace MyTelegram.AuthServer.Services;

public record Step2Output(byte[] NewNonce, IServerDHParams ServerDhParams);