namespace MyTelegram.Services.Exceptions;

public class BadRequestException(string errorMessage) : RpcException(400, errorMessage);