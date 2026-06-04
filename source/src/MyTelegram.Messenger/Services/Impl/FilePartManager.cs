using System.Net.Http.Json;

namespace MyTelegram.Messenger.Services.Impl;

public class FilePartManager(
    ILogger<FilePartManager> logger,
    IOptions<MyTelegramMessengerServerOptions> options,
    IHttpClientFactory httpClientFactory) : IFilePartManager, ITransientDependency
{
    private readonly string _tempPath = Path.Combine(options.Value.FileServerUploadPath ?? "uploads", "temp");
    private readonly string _mergedPath = Path.Combine(options.Value.FileServerUploadPath ?? "uploads", "merged");
    private readonly string _fileServerUrl = "http://file-merge-proxy:5000";

    public async Task<(string FilePath, long TotalSize)> MergeFilePartsAsync(long userId, long fileId, int parts, string name)
    {
        try
        {
            // Call file-server HTTP API to merge file parts
            var httpClient = httpClientFactory.CreateClient();
            var requestUrl = $"{_fileServerUrl}/api/files/merge";
            
            var requestData = new
            {
                userId = userId,
                fileId = fileId,
                parts = parts,
                fileName = name
            };

            logger.LogInformation(
                "Requesting file-server to merge file parts: userId={UserId}, fileId={FileId}, parts={Parts}, url={Url}",
                userId, fileId, parts, requestUrl);

            var response = await httpClient.PostAsJsonAsync(requestUrl, requestData);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogError(
                    "File-server merge request failed: {StatusCode}, {Error}",
                    response.StatusCode, errorContent);
                throw new Exception($"File-server merge failed: {response.StatusCode}");
            }

            var result = await response.Content.ReadFromJsonAsync<MergeFileResponse>();
            
            if (result == null)
            {
                throw new Exception("File-server returned null response");
            }

            // Convert relative path to absolute path
            var basePath = options.Value.FileServerUploadPath ?? "/app/uploads";
            var absolutePath = Path.Combine(basePath, result.FilePath);
            
            logger.LogInformation(
                "File-server merged file successfully: filePath={FilePath}, absolutePath={AbsolutePath}, size={Size}",
                result.FilePath, absolutePath, result.TotalSize);

            return (absolutePath, result.TotalSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to merge file parts via file-server for fileId: {FileId}", fileId);
            throw;
        }
    }

    private class MergeFileResponse
    {
        public string FilePath { get; set; } = "";
        public long TotalSize { get; set; }
    }

    public async Task CleanupFilePartsAsync(long userId, long fileId)
    {
        try
        {
            // Delete temp parts directory
            var userFileDir = Path.Combine(_tempPath, $"{userId}-{fileId}");
            if (Directory.Exists(userFileDir))
            {
                Directory.Delete(userFileDir, true);
            }

            // Delete merged file
            var mergedPattern = $"{userId}-{fileId}.*";
            var mergedDir = new DirectoryInfo(_mergedPath);
            if (mergedDir.Exists)
            {
                foreach (var file in mergedDir.GetFiles(mergedPattern))
                {
                    file.Delete();
                }
            }

            logger.LogInformation("Cleaned up file parts for userId: {UserId}, fileId: {FileId}", userId, fileId);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to cleanup file parts for userId: {UserId}, fileId: {FileId}", userId, fileId);
        }

        await Task.CompletedTask;
    }

    public bool ValidateFileParts(long userId, long fileId, int parts)
    {
        var userFileDir = Path.Combine(_tempPath, $"{userId}-{fileId}");
        
        if (!Directory.Exists(userFileDir))
        {
            return false;
        }

        for (int i = 0; i < parts; i++)
        {
            var partPath = Path.Combine(userFileDir, $"part_{i}");
            if (!File.Exists(partPath))
            {
                return false;
            }
        }

        return true;
    }
}
