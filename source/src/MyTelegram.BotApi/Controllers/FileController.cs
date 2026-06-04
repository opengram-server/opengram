using Microsoft.AspNetCore.Mvc;

namespace MyTelegram.BotApi.Controllers;

[ApiController]
[Route("file/bot{token}")]
public class FileController(ILogger<FileController> logger) : ControllerBase
{
    [HttpGet("{filePath}")]
    public async Task<IActionResult> DownloadFile([FromRoute] string token, [FromRoute] string filePath)
    {
        logger.LogInformation("File download request: {FilePath}", filePath);
        
        // TODO: Implement file download from MinIO
        return NotFound(new { ok = false, error_code = 404, description = "File not found" });
    }
}
