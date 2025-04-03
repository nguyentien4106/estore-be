using Estore.Application.Helpers;
using Estore.Application.Services;

namespace Estore.Application.Files.Commands.UploadFile;

public class UploadFileHandler(
    ICloudflareClient client,
    IEStoreDbContext context,
    UserManager<User> userManager
    ) : ICommandHandler<UploadFileCommand, AppResponse<FileInformationDto>>
{
    public async Task<AppResponse<FileInformationDto>> Handle(UploadFileCommand command, CancellationToken cancellationToken)
    {
        var file = command.File;
        using Stream fileStream = file.OpenReadStream();
        var storageFileName = FileHelper.CreateStorageFileName(command.UserName, file.FileName);
        var result = await client.UploadFileAsync(storageFileName, fileStream, file.ContentType);

        if (!result.Succeed)
        {
            return AppResponse<FileInformationDto>.Error(result.Message);
        }

        var user = await userManager.FindByNameAsync(command.UserName);
        if (user is null)
        {
            return AppResponse<FileInformationDto>.NotFound("User", command.UserName);
        }

        var fileInformation = R2FileInformation.Create(
            userId: Guid.Parse(user.Id),
            fileName: file.FileName,
            fileSize: FileHelper.GetFileSizeInMb(file.Length),
            url: result.Data.Url,
            fileType: FileHelper.DetermineFileType(file.FileName),
            storageFileName
        );

        await context.R2Files.AddAsync(fileInformation);
        await context.CommitAsync(cancellationToken);
        var dto = fileInformation.Adapt<FileInformationDto>();

        return AppResponse<FileInformationDto>.Success(dto);
    }
}
