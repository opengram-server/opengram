namespace MyTelegram.AuthServer.Services;

public record Step1Output(byte[] P, byte[] Q, byte[] ServerNonce, TResPQ ResPq);