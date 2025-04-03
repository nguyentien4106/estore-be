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
        var user = await userManager.FindByNameAsync(command.UserName);
        if (user is null)
        {
            return AppResponse<FileInformationDto>.NotFound("User", command.UserName);
        }

        var result = await telegramService.UploadFileToStrorageAsync(file, user.Id);

        if (!result.Succeed)
        {
            return AppResponse<FileInformationDto>.Error(result.Message);
        }

        var fileInformation = TelegramFileInformation.Create(
            userId: Guid.Parse(user.Id),
            fileName: file.FileName,
            fileSize: FileHelper.GetFileSizeInMb(file.Length),
            fileType: FileHelper.DetermineFileType(file.FileName),
            "localPath",
            "fileId"
        );

        await context.TeleFiles.AddAsync(fileInformation);
        await context.CommitAsync(cancellationToken);
        var dto = fileInformation.Adapt<FileInformationDto>();

        return AppResponse<FileInformationDto>.Success(dto);
    }
}
