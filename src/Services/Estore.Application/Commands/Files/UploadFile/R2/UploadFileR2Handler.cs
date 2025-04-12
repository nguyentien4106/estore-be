using EStore.Application.Helpers;
using EStore.Application.Services;
using EStore.Application.Services.Cloudflare;
using EStore.Domain.Models.Base;
using EStore.Domain.Extensions;
using EStore.Application.Extensions;
using EStore.Application.Models.Files;

namespace EStore.Application.Commands.Files.UploadFile;

public class UploadFileR2Handler(
    ICloudflareClient r2,
    IEStoreDbContext context,
    UserManager<User> userManager
    ) : ICommandHandler<UploadFileR2Command, AppResponse<FileEntityResult>>
{
    public async Task<AppResponse<FileEntityResult>> Handle(UploadFileR2Command command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(command.UserName);
        if (user is null)
        {
            return AppResponse<FileEntityResult>.NotFound("User", command.UserName);
        }
        
        var result = await r2.UploadFileAsync(command.File, user.UserName);
        if(!result.Succeed || result.Data is null){
            return AppResponse<FileEntityResult>.Error(result.Message);
        }

        R2FileEntity r2File = result.Data?.ToR2FileEntity() ?? new();
        r2File.FileKey = R2Helper.GetR2FileKey(user.UserName, command.File.FileName);
        r2File.UserId = user.Id;

        await context.R2FileEntities.AddAsync(r2File, cancellationToken);
        await context.CommitAsync(cancellationToken);

        return AppResponse<FileEntityResult>.Success(r2File.ToFileEntityResponse());
    }
}
