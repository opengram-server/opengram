using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyTelegram.BotApi.Options;

namespace MyTelegram.BotApi.Controllers;

[ApiController]
[Route("file/bot{token}")]
public class FileController(
    ILogger<FileController> logger,
    IOptions<MinioOptions> minioOptions) : ControllerBase
{
    private static readonly HttpClient HttpClient = new();
    private readonly MinioOptions _minioOptions = minioOptions.Value;

    /// <summary>
    /// Download file from MinIO storage.
    /// File path format: documents/{fileId} or photos/{fileId}
    /// </summary>
    [HttpGet("{**filePath}")]
    public async Task<IActionResult> DownloadFile([FromRoute] string token, [FromRoute] string filePath)
    {
        logger.LogInformation("File download request: {FilePath}", filePath);

        if (string.IsNullOrEmpty(filePath))
        {
            return NotFound(new { ok = false, error_code = 404, description = "File path is required" });
        }

        try
        {
            // Extract the file ID from the path (documents/123456 or photos/123456)
            var fileId = filePath;
            if (filePath.Contains('/'))
            {
                fileId = filePath.Split('/').Last();
            }

            var minioEndpoint = _minioOptions.Endpoint ?? "minio:9000";
            var bucketName = _minioOptions.BucketName ?? "tg-files";
            var minioUrl = $"http://{minioEndpoint}/{bucketName}/{fileId}";

            logger.LogInformation("Fetching file from MinIO: {Url}", minioUrl);

            var response = await HttpClient.GetAsync(minioUrl, HttpContext.RequestAborted);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("MinIO returned {StatusCode} for file {FileId}", response.StatusCode, fileId);
                return NotFound(new { ok = false, error_code = 404, description = "File not found" });
            }

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var fileStream = await response.Content.ReadAsStreamAsync(HttpContext.RequestAborted);

            return File(fileStream, contentType);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to fetch file from MinIO: {FilePath}", filePath);
            return StatusCode(502, new { ok = false, error_code = 502, description = "Storage unavailable" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error downloading file: {FilePath}", filePath);
            return StatusCode(500, new { ok = false, error_code = 500, description = "Internal server error" });
        }
    }
}
