// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Upload;

///<summary>
/// Saves a part of a large file (over 10 MB in size) to be later passed to one of the methods.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 FILE_PARTS_INVALID The number of file parts is invalid.
/// 400 FILE_PART_EMPTY The provided file part is empty.
/// 400 FILE_PART_INVALID The file part number is invalid.
/// 400 FILE_PART_SIZE_CHANGED Provided file part size has changed.
/// 400 FILE_PART_SIZE_INVALID The provided file part size is invalid.
/// 400 FILE_PART_TOO_BIG The uploaded file part is too big.
/// See <a href="https://corefork.telegram.org/method/upload.saveBigFilePart" />
///</summary>
internal sealed class SaveBigFilePartHandler(
    ILogger<SaveBigFilePartHandler> logger,
    IOptions<MyTelegramMessengerServerOptions> options)
    : RpcResultObjectHandler<MyTelegram.Schema.Upload.RequestSaveBigFilePart, IBool>,
    Upload.ISaveBigFilePartHandler
{
    private readonly string _uploadsPath = Path.Combine(options.Value.FileServerUploadPath ?? "uploads", "temp");

    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Upload.RequestSaveBigFilePart obj)
    {
        try
        {
            // Ограничиваем размер одной части файла (не более 512 КБ)
            if (obj.Bytes.Length > 524288) // максимум 512 КБ на часть
            {
                logger.LogWarning("File part too big - Size: {Size}, User: {UserId}",
                    obj.Bytes.Length, input.UserId);
                RpcErrors.RpcErrors400.FilePartTooBig.ThrowRpcError();
            }

            // Ограничиваем общее число частей (до 10 000 частей, то есть примерно 5 ГБ)
            if (obj.FileTotalParts > 10000)
            {
                logger.LogWarning("Too many file parts - Parts: {Parts}, User: {UserId}",
                    obj.FileTotalParts, input.UserId);
                RpcErrors.RpcErrors400.FilePartsInvalid.ThrowRpcError();
            }

            // Создаём временную директорию для загрузок
            Directory.CreateDirectory(_uploadsPath);

            // Части файла храним по пути userId-fileId/part_N
            var userFileDir = Path.Combine(_uploadsPath, $"{input.UserId}-{obj.FileId}");
            Directory.CreateDirectory(userFileDir);
            
            var partFilePath = Path.Combine(userFileDir, $"part_{obj.FilePart}");

            // Сохраняем часть файла на диск
            await File.WriteAllBytesAsync(partFilePath, obj.Bytes.ToArray());
            
            logger.LogInformation(
                "Save BIG file part ({BytesCount} bytes), fileId: {FileId}, partId: {PartId}, totalParts: {TotalParts}",
                obj.Bytes.Length, obj.FileId, obj.FilePart, obj.FileTotalParts);
            
            return new TBoolTrue();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save big file part, fileId: {FileId}, partId: {PartId}", 
                obj.FileId, obj.FilePart);
            throw;
        }
    }
}
