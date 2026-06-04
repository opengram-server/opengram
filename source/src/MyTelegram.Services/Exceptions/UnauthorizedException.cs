namespace MyTelegram.Services.Exceptions;

public class UnauthorizedException(string errorMessage) : RpcException(401, errorMessage);