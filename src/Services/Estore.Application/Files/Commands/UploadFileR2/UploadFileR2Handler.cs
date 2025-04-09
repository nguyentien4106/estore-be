using EStore.Application.Helpers;
using EStore.Application.Services;
using EStore.Application.Services.Cloudflare;
using EStore.Domain.Models.Base;
using EStore.Domain.Extensions;

namespace EStore.Application.Files.Commands.UploadFileR2;

public class UploadFileR2Handler(
    ICloudflareClient r2,
    IEStoreDbContext context,
    UserManager<User> userManager
    ) : ICommandHandler<UploadFileR2Command, AppResponse<FileEntity>>
{
    public async Task<AppResponse<FileEntity>> Handle(UploadFileR2Command command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(command.UserName);
        if (user is null)
        {
            return AppResponse<FileEntity>.NotFound("User", command.UserName);
        }
        
        var result = await r2.UploadFileAsync(command.File, user.UserName);
        if(!result.Succeed || result.Data is null){
            return AppResponse<FileEntity>.Error(result.Message);
        }

        R2FileEntity r2File = result.Data?.ToR2FileEntity() ?? new();
        r2File.FileKey = R2Helper.GetR2FileKey(user.UserName, command.File.FileName);
        r2File.UserId = user.Id;

        await context.R2FileEntities.AddAsync(r2File, cancellationToken);
        await context.CommitAsync(cancellationToken);

        return AppResponse<FileEntity>.Success(r2File);
    }
}
