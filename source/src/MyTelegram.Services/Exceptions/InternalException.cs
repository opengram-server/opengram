namespace MyTelegram.Services.Exceptions;

public class InternalException(string errorMessage) : RpcException(500, errorMessage);