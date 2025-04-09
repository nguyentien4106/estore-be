using Microsoft.AspNetCore.Authorization;

namespace EStore.Api.Middlewares;
public class FileSizeLimitRequirement : IAuthorizationRequirement
{
    public long MaxFileSize { get; }

    public FileSizeLimitRequirement(long maxFileSize)
    {
        MaxFileSize = maxFileSize;
    }
}