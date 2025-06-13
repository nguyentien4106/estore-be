using Microsoft.AspNetCore.Http;

namespace EStore.Application.Helpers;

public static class FileHelper
{
    private static readonly Dictionary<string, FileType> ExtensionToFileType = new(StringComparer.OrdinalIgnoreCase)
    {
        // Images
        { ".jpg", FileType.Image },
        { ".jpeg", FileType.Image },
        { ".png", FileType.Image },
        { ".gif", FileType.Image },
        { ".bmp", FileType.Image },
        { ".webp", FileType.Image },
        { ".svg", FileType.Image },
        { ".ico", FileType.Image },

        // Text
        { ".txt", FileType.Text },
        { ".csv", FileType.Text },
        { ".json", FileType.Text },
        { ".xml", FileType.Text },
        { ".html", FileType.Text },
        { ".htm", FileType.Text },
        { ".css", FileType.Text },
        { ".js", FileType.Text },
        { ".md", FileType.Text },
        { ".log", FileType.Text },

        // Documents
        { ".pdf", FileType.Document },
        { ".doc", FileType.Document },
        { ".docx", FileType.Document },
        { ".xls", FileType.Document },
        { ".xlsx", FileType.Document },
        { ".ppt", FileType.Document },
        { ".pptx", FileType.Document },
        { ".odt", FileType.Document },
        { ".ods", FileType.Document },
        { ".odp", FileType.Document },

        // Audio
        { ".mp3", FileType.Audio },
        { ".wav", FileType.Audio },
        { ".ogg", FileType.Audio },
        { ".m4a", FileType.Audio },
        { ".flac", FileType.Audio },
        { ".aac", FileType.Audio },

        // Video
        { ".mp4", FileType.Video },
        { ".avi", FileType.Video },
        { ".mov", FileType.Video },
        { ".wmv", FileType.Video },
        { ".flv", FileType.Video },
        { ".mkv", FileType.Video },
        { ".webm", FileType.Video },

        // Compressed
        { ".zip", FileType.Compressed },
        { ".rar", FileType.Compressed },
        { ".7z", FileType.Compressed },
        { ".tar", FileType.Compressed },
        { ".gz", FileType.Compressed },
        { ".bz2", FileType.Compressed },

        // System
        { ".exe", FileType.System },
        { ".dll", FileType.System },
        { ".sys", FileType.System },
        { ".ini", FileType.System },
        { ".config", FileType.System }
    };

    /// <summary>
    /// Determines the FileType based on the file name extension
    /// </summary>
    /// <param name="fileName">The name of the file including extension</param>
    /// <returns>The determined FileType, or FileType.Unknown if the extension is not recognized</returns>
    public static FileType DetermineFileType(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return FileType.Unknown;

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return ExtensionToFileType.TryGetValue(extension, out var fileType) 
            ? fileType 
            : FileType.Unknown;
    }

    /// <summary>   
    /// Converts file size in bytes to megabytes
    /// </summary>
    /// <param name="bytes">The file size in bytes</param>
    /// <returns>The file size in megabytes as a decimal value</returns>
    public static decimal GetFileSizeInMb(long bytes)
    {
        const decimal bytesInMb = 1024 * 1024;
        return Math.Round(bytes / bytesInMb, 2);
    }

    /// <summary>
    /// Creates a storage file name for a given user and file name => userName/fileName
    /// </summary>
    /// <param name="userName">The name of the user</param>
    /// <param name="fileName">The name of the file</param>
    /// <returns>The storage file name</returns>
    public static string CreateStorageFileName(string userName, string fileName) => $"{userName}/{fileName}";

    /// <summary>
    /// Creates a temporary file from an IFormFile and returns the file path
    /// </summary>
    /// <param name="file">The IFormFile to create a temporary file from</param>
    /// <returns>The path to the temporary file</returns>
    public static async Task<string> CreateTempFileAsync(IFormFile file)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), file.FileName);
        
        using (var stream = new FileStream(tempPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return tempPath;
    }

    /// <summary>
    /// Get the stream of the file
    /// </summary>
    /// <param name="file">The file to get the stream from</param>
    /// <returns>The stream of the file</returns>
    public static Stream GetMemoryStream(IFormFile file)
    {
        var stream = new MemoryStream();
        file.CopyTo(stream);
        stream.Position = 0;
        return stream;
    }

    public static Stream GetFileStream(IFormFile file)
    {
        return file.OpenReadStream();
    }


    public static string GetFileExtension(string fileName)
    {
        return Path.GetExtension(fileName).TrimStart('.');
    }

    public static string GetTempsFilePath(string userId, string fileId, int chunkIndex = -1){
        var filePath = chunkIndex != -1 ? Path.Combine(AppContext.BaseDirectory, "temps", userId, fileId, chunkIndex.ToString()) 
                                        : Path.Combine(AppContext.BaseDirectory, "temps", userId, fileId);
        var directoryPath = Path.GetDirectoryName(filePath);

        if (directoryPath != null && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        return filePath;
    }

    public static string GetTempFileDownloadPath(string userId, string fileName){
        var filePath = Path.Combine(AppContext.BaseDirectory, "downloads", userId, fileName);
        if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "downloads", userId)))
        {
            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "downloads", userId));
        }

        return filePath;
    }

    public static string GetMimeTypeTelegram(string? fileName){
        return Path.GetExtension(fileName)?.ToLowerInvariant() switch
			{
				".jpg" or ".jpeg" or ".png" or ".bmp" => "photo",
				".mp4" => "video",
				".gif" => "image/gif",
				".webp" => "image/webp",
				".mp3" => "audio/mpeg",
				".wav" => "audio/x-wav",
				_ => "",
			};
    }
} 