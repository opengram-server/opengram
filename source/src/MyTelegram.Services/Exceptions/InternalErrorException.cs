namespace MyTelegram.Services.Exceptions;

public class InternalErrorException(string errorMessage) : RpcException(500, errorMessage);