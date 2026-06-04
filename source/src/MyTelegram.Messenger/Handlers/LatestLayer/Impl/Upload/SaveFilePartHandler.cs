// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Upload;

///<summary>
/// Saves a part of file for further sending to one of the methods.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 FILE_PART_EMPTY The provided file part is empty.
/// 400 FILE_PART_INVALID The file part number is invalid.
/// 400 MSG_ID_INVALID Invalid message ID provided.
/// See <a href="https://corefork.telegram.org/method/upload.saveFilePart" />
///</summary>
internal sealed class SaveFilePartHandler(
    ILogger<SaveFilePartHandler> logger,
    IOptions<MyTelegramMessengerServerOptions> options) 
    : RpcResultObjectHandler<MyTelegram.Schema.Upload.RequestSaveFilePart, IBool>,
    Upload.ISaveFilePartHandler
{
    private readonly string _uploadsPath = Path.Combine(options.Value.FileServerUploadPath ?? "uploads", "temp");

    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Upload.RequestSaveFilePart obj)
    {
        try
        {
            // Create temp directory if not exists
            Directory.CreateDirectory(_uploadsPath);
            
            // File part storage: userId-fileId/part_N
            var userFileDir = Path.Combine(_uploadsPath, $"{input.UserId}-{obj.FileId}");
            Directory.CreateDirectory(userFileDir);
            
            var partFilePath = Path.Combine(userFileDir, $"part_{obj.FilePart}");
            
            // Save file part to disk
            await File.WriteAllBytesAsync(partFilePath, obj.Bytes.ToArray());
            
            logger.LogInformation(
                "Save file part ({BytesCount} bytes), reqMsgId: {ReqMsgId}, fileId: {FileId}, partId: {PartId}",
                obj.Bytes.Length, input.ReqMsgId, obj.FileId, obj.FilePart);
            
            return new TBoolTrue();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save file part, fileId: {FileId}, partId: {PartId}", 
                obj.FileId, obj.FilePart);
            throw;
        }
    }
}
