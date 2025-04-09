using BuildingBlocks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EStore.Api.Middlewares;

public class AuthorizationFailureMetadata
{
    public string Reason { get; set; }
    public string Message { get; set; }

    public AuthorizationFailureMetadata(string reason, string message)
    {
        Reason = reason;
        Message = message;
    }
}

public class FileSizeLimitAuthorizationHandler : AuthorizationHandler<FileSizeLimitRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileSizeLimitAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FileSizeLimitRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            context.Fail(new AuthorizationFailureReason(this, "Unable to access HTTP context."));
            return Task.CompletedTask;
        }

        if (httpContext.Request.HasFormContentType)
        {
            var form = httpContext.Request.Form;
            long totalSize = 0;

            if (form.Files != null && form.Files.Count > 0)
            {
                totalSize = form.Files.Sum(file => file.Length);
            }

            if (totalSize > requirement.MaxFileSize)
            {
                context.Fail(new AuthorizationFailureReason(this, "File size exceeds the allowed limit for your tier."));
                httpContext.Items["AuthorizationFailure"] = new AppResponse<string>(){
                    Succeed = false,
                    Data = "FILE_SIZE_LIMIT_EXCEEDED",
                    Message = "File size exceeds the allowed limit for your tier."
                };

                return Task.CompletedTask;
            }
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}