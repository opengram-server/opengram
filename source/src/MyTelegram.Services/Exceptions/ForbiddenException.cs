namespace MyTelegram.Services.Exceptions;

public class ForbiddenException(string errorMessage) : RpcException(403, errorMessage);