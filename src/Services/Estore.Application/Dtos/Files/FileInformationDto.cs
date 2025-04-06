namespace EStore.Application.Dtos.Files;

public record FileInformationDto(
    Guid Id,
    string FileName,
    string StorageFileName,
    decimal FileSize,
    string Url, 
    FileType FileType, 
    StorageSource StorageSource, 
    Guid UserId, 
    DateTime CreatedAt
);
