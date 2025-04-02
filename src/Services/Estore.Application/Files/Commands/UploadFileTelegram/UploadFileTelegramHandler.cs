using Estore.Application.Helpers;
using Estore.Application.Services;

namespace Estore.Application.Files.Commands.UploadFileTelegram;

public class UploadFileTelegramHandler(
    IEStoreDbContext context,
    UserManager<User> userManager,
    ITelegramService telegramService
    ) : ICommandHandler<UploadFileTelegramCommand, AppResponse<FileInformationDto>>
{
    public async Task<AppResponse<FileInformationDto>> Handle(UploadFileTelegramCommand command, CancellationToken cancellationToken)
    {
        var file = command.File;
        var result = await telegramService.UploadFileToStrorageAsync(file, "Caption " + command.UserName);

        if (!result.Succeed)
        {
            return AppResponse<FileInformationDto>.Error(result.Message);
        }

        var user = await userManager.FindByNameAsync(command.UserName);
        if (user is null)
        {
            return AppResponse<FileInformationDto>.NotFound("User", command.UserName);
        }

        var fileInformation = FileInformation.Create(
            userId: Guid.Parse(user.Id),
            fileName: file.FileName,
            fileSize: FileHelper.GetFileSizeInMb(file.Length),
            url: result.Data.ToString(),
            fileType: FileHelper.DetermineFileType(file.FileName),
            storageSource: StorageSource.Telegram,
            storageFileName: result.Data.ToString()
        );

        await context.Files.AddAsync(fileInformation);
        await context.CommitAsync(cancellationToken);
        var dto = fileInformation.Adapt<FileInformationDto>();

        return AppResponse<FileInformationDto>.Success(dto);
    }
}
