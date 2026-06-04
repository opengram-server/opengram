namespace MyTelegram.Messenger.Services.Interfaces;

public interface IFilePartManager
{
    /// <summary>
    /// Merge file parts into a single file
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="fileId">File ID</param>
    /// <param name="parts">Total number of parts</param>
    /// <param name="name">File name</param>
    /// <returns>Path to merged file and total size</returns>
    Task<(string FilePath, long TotalSize)> MergeFilePartsAsync(long userId, long fileId, int parts, string name);
    
    /// <summary>
    /// Clean up temporary file parts
    /// </summary>
    Task CleanupFilePartsAsync(long userId, long fileId);
    
    /// <summary>
    /// Check if all file parts exist
    /// </summary>
    bool ValidateFileParts(long userId, long fileId, int parts);
}
