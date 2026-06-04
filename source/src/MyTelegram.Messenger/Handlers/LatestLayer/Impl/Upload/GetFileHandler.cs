// ReSharper disable All

using System.Net.Http;
using Microsoft.Extensions.Options;
using MyTelegram.Domain.Options;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Upload;

///<summary>
/// Returns content of a whole file or its part.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHANNEL_INVALID The provided channel is invalid.
/// 400 CHANNEL_PRIVATE You haven't joined this channel/supergroup.
/// 400 FILE_REFERENCE_* The file reference expired, it <a href="https://corefork.telegram.org/api/file_reference">must be refreshed</a>.
/// 406 FILEREF_UPGRADE_NEEDED The client has to be updated in order to support <a href="https://corefork.telegram.org/api/file_reference">file references</a>.
/// 400 FILE_ID_INVALID The provided file id is invalid.
/// 400 FILE_REFERENCE_EXPIRED File reference expired, it must be refetched as described in <a href="https://corefork.telegram.org/api/file_reference">the documentation</a>.
/// 400 LIMIT_INVALID The provided limit is invalid.
/// 400 LOCATION_INVALID The provided location is invalid.
/// 400 MSG_ID_INVALID Invalid message ID provided.
/// 400 OFFSET_INVALID The provided offset is invalid.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// See <a href="https://corefork.telegram.org/method/upload.getFile" />
///</summary>
internal sealed class GetFileHandler : RpcResultObjectHandler<MyTelegram.Schema.Upload.RequestGetFile, MyTelegram.Schema.Upload.IFile>,
    Upload.IGetFileHandler
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly ILogger<GetFileHandler> _logger;
    private readonly MinioOptions _minioOptions;
    private static readonly HttpClient _httpClient = new HttpClient();

    public GetFileHandler(
        IQueryProcessor queryProcessor,
        ILogger<GetFileHandler> logger,
        IOptions<MinioOptions> minioOptions)
    {
        _queryProcessor = queryProcessor;
        _logger = logger;
        _minioOptions = minioOptions.Value;
    }

    protected override async Task<MyTelegram.Schema.Upload.IFile> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Upload.RequestGetFile obj)
    {
        // Определяем идентификатор файла и тип запроса: фото или документ
        long fileId = 0;
        bool isPhoto = false;
        int mtime = 0;
        string? thumbSize = null;  // Размер превью для премиум-эффектов стикеров (type="f")

        if (obj.Location is TInputDocumentFileLocation docLocation)
        {
            fileId = docLocation.Id;
            thumbSize = docLocation.ThumbSize;  // Берём thumb_size из запроса
            _logger.LogInformation("Loading document: DocumentId={DocumentId}, ThumbSize={ThumbSize}", 
                fileId, thumbSize ?? "null");
        }
        else if (obj.Location is TInputPeerPhotoFileLocation peerPhotoLocation)
        {
            // Для фото пиров (аватарок) используем PhotoId
            fileId = peerPhotoLocation.PhotoId;
            isPhoto = true;
            _logger.LogInformation("Loading peer photo: PhotoId={PhotoId}, Peer={Peer}", 
                peerPhotoLocation.PhotoId, peerPhotoLocation.Peer?.GetType().Name);
        }
        else if (obj.Location is TInputPhotoFileLocation photoLocation)
        {
            // Для фотографий (например, фото профиля) используем Id
            fileId = photoLocation.Id;
            isPhoto = true;
            _logger.LogInformation("Loading photo: PhotoId={PhotoId}, ThumbSize={ThumbSize}", 
                photoLocation.Id, photoLocation.ThumbSize);
        }
        else
        {
            _logger.LogWarning("Unsupported file location type: {Type}", obj.Location?.GetType().Name);
            RpcErrors.RpcErrors400.LocationInvalid.ThrowRpcError();
        }

        _logger.LogInformation("GetFile: FileId={FileId}, IsPhoto={IsPhoto}, Offset={Offset}, Limit={Limit}", 
            fileId, isPhoto, obj.Offset, obj.Limit);

        // Если это фото, сначала пробуем загрузить его из PhotoReadModel
        if (isPhoto)
        {
            var photoQuery = new GetPhotoByIdQuery(fileId);
            var photo = await _queryProcessor.ProcessAsync(photoQuery, default);
            
            if (photo != null)
            {
                mtime = photo.Date;
                _logger.LogInformation("Photo found in PhotoReadModel: PhotoId={PhotoId}, Date={Date}", 
                    fileId, photo.Date);
            }
            else
            {
                _logger.LogWarning("Photo not found in PhotoReadModel: {PhotoId}, trying DocumentReadModel", fileId);
            }
        }

        // Если в PhotoReadModel ничего не нашли, пробуем DocumentReadModel
        if (mtime == 0)
        {
            var query = new GetDocumentByIdQuery(fileId);
            var document = await _queryProcessor.ProcessAsync(query, default);

            if (document == null)
            {
                _logger.LogWarning("File not found in both PhotoReadModel and DocumentReadModel: {FileId}", fileId);
                RpcErrors.RpcErrors400.FileIdInvalid.ThrowRpcError();
            }
            
            mtime = document.Date;
            _logger.LogInformation("Document found in DocumentReadModel: DocumentId={DocumentId}, Date={Date}", 
                fileId, document.Date);
            
            // Премиум-стикеры: при thumb_size="f" загружаем эффект из VideoThumbId
            if (!string.IsNullOrEmpty(thumbSize) && thumbSize == "f")
            {
                if (document.VideoThumbId.HasValue && document.VideoThumbId.Value > 0)
                {
                    fileId = document.VideoThumbId.Value;
                    _logger.LogInformation("Premium effect requested (thumb_size=f), loading VideoThumbId={VideoThumbId}",
                        fileId);
                }
                else
                {
                    _logger.LogWarning("Premium effect requested but VideoThumbId is null for DocumentId={DocumentId}",
                        document.DocumentId);
                    RpcErrors.RpcErrors400.FileIdInvalid.ThrowRpcError();
                }
            }
        }

        // file-server ожидает файлы в корне бакета, без подпапок и расширений.
        // Имя объекта совпадает с {fileId}
        var objectName = $"{fileId}";

        _logger.LogInformation("Loading file from MinIO: {ObjectName}", objectName);

        try
        {
            // Load file from MinIO via HTTP
            // Use MinIO endpoint from configuration (e.g., "minio:9000" in Docker)
            var minioEndpoint = _minioOptions.Endpoint ?? "minio:9000";
            var bucketName = _minioOptions.BucketName ?? "tg-files";
            var minioUrl = $"http://{minioEndpoint}/{bucketName}/{objectName}";
            
            _logger.LogInformation("Fetching file from MinIO: {Url}", minioUrl);
            
            var request = new HttpRequestMessage(HttpMethod.Get, minioUrl);
            
            // Only add Range header if offset > 0 or limit is specified
            if (obj.Offset > 0 || obj.Limit < int.MaxValue)
            {
                var endByte = obj.Offset + obj.Limit - 1;
                request.Headers.Add("Range", $"bytes={obj.Offset}-{endByte}");
            }
            
            var response = await _httpClient.SendAsync(request, default);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to load file from MinIO: {StatusCode}, URL: {Url}", 
                    response.StatusCode, minioUrl);
                RpcErrors.RpcErrors400.FileIdInvalid.ThrowRpcError();
            }

            var fileData = await response.Content.ReadAsByteArrayAsync(default);
            
            _logger.LogInformation("File loaded successfully: {Size} bytes from {ObjectName}", 
                fileData.Length, objectName);

            // Return file data
            return new MyTelegram.Schema.Upload.TFile
            {
                Type = new MyTelegram.Schema.Storage.TFilePartial(),
                Mtime = mtime,
                Bytes = fileData
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading file from MinIO: {ObjectName}", objectName);
            RpcErrors.RpcErrors400.FileIdInvalid.ThrowRpcError();
            return null!;
        }
    }
}
